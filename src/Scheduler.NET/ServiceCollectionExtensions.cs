using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.NET.Filter;
using Scheduler.NET.JobManager;
using Scheduler.NET.Common;
using System;
using Hangfire.MySql;
using Hangfire.MySql.Core;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MySql.Data.MySqlClient;

namespace Scheduler.NET
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 必须放在UseMvc前面
		/// </summary>
		/// <param name="app"></param>
		public static void UseScheduler(this IApplicationBuilder app)
		{
			app.UseSignalR(routes =>
			{
				routes.MapHub<ClientHub>("/client");
			});
			app.UseHangfireServer();
			app.UseHangfireDashboard("/hangfire", new DashboardOptions()
			{
				Authorization = new[] { new CustomAuthorizeFilter() }
			});
		}

		public static IMvcBuilder AddScheduler(this IMvcBuilder builder, Action<ISchedulerOptions> setupAction)
		{
			var schedulerOptions = new SchedulerOptions();
			setupAction(schedulerOptions);

			builder.Services.AddHttpClient();
			builder.Services.AddSignalR();

			builder.AddMvcOptions(options => options.Filters.Add<HttpGlobalExceptionFilter>());

			builder.Services.AddSingleton<ISchedulerOptions>(schedulerOptions);
			builder.Services.AddTransient<IJobManager<CallbackJob>, HangFireCallbackJobManager>();
			builder.Services.AddTransient<IJobManager<Job>, HangFireJobManager>();

			switch (schedulerOptions.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						builder.Services.AddHangfire(r => r.UseSqlServerStorage(schedulerOptions.HangfireConnectionString));
						break;
					}
				case "redis":
					{
						builder.Services.AddHangfire(r => r.UseRedisStorage(schedulerOptions.HangfireConnectionString));
						break;
					}
				case "mysql":
					{
						builder.Services.AddHangfire(r => { });
						GlobalConfiguration.Configuration.UseStorage(
							new MySqlStorage(
								schedulerOptions.HangfireConnectionString,
								new MySqlStorageOptions
								{
									TransactionIsolationLevel = IsolationLevel.ReadCommitted,
									QueuePollInterval = TimeSpan.FromSeconds(5),
									JobExpirationCheckInterval = TimeSpan.FromHours(1),
									CountersAggregateInterval = TimeSpan.FromMinutes(5),
									PrepareSchemaIfNecessary = true,
									DashboardJobListLimit = 50000,
									TransactionTimeout = TimeSpan.FromMinutes(1),
								}));
						break;
					}
			}

			InitDatabase(schedulerOptions);
			return builder;
		}

		public static IMvcBuilder AddScheduler(this IMvcBuilder builder, IConfiguration configuration)
		{
			var section = configuration.GetSection(SchedulerOptions.DefaultSettingKey);
			var schedulerOptions = section.Get<SchedulerOptions>();

			return builder.AddScheduler(options =>
			{
				options.HangfireConnectionString = schedulerOptions.HangfireConnectionString;
				options.HangfireStorageType = schedulerOptions.HangfireStorageType;
				options.IgnoreCrons = schedulerOptions.IgnoreCrons;
				options.TokenHeader = schedulerOptions.TokenHeader;
				options.Tokens = schedulerOptions.Tokens;
				options.UseToken = schedulerOptions.UseToken;
			});
		}

		private static void InitDatabase(ISchedulerOptions schedulerOptions)
		{
			switch (schedulerOptions.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						using (var conn = new SqlConnection(schedulerOptions.HangfireConnectionString))
						{
							try
							{
								conn.Execute(@"CREATE TABLE scheduler_job (
  id varchar(32) NOT NULL,
  [group] varchar(255) NOT NULL,
  classname  varchar(255) NOT NULL,
  cron  varchar(50) NOT NULL,
  content  varchar(500) DEFAULT NULL,
  isEnable bit,
  status int,
  creationtime DateTime NOT NULL,
  lastmodificationtime DateTime,
  PRIMARY KEY(id)
) ");
							}
							catch (Exception e) when (e.Message.Contains("数据库中已存在名为 'scheduler_job' 的对象。") || e.Message.Contains(""))
							{
								// IGNORE
							}
							try
							{
								conn.Execute(@"CREATE TABLE scheduler_job_history (
  batchid varchar(32) NOT NULL,
  jobid varchar(32) NOT NULL,
  status int,
  creationtime DateTime NOT NULL,
  lastmodificationtime DateTime,
  PRIMARY KEY(batchid,jobid)
)");
							}
							catch (Exception e) when (e.Message.Contains("数据库中已存在名为 'scheduler_job_history' 的对象。") || e.Message.Contains(""))
							{
								// IGNORE
							}
						}

						break;
					}
				case "mysql":
					{
						using (var conn = new MySqlConnection(schedulerOptions.HangfireConnectionString))
						{
							conn.Execute(@"CREATE TABLE IF NOT EXISTS scheduler_job (
  id varchar(32) NOT NULL,
  `group` varchar(255) NOT NULL,
  classname  varchar(255) NOT NULL,
  cron  varchar(50) NOT NULL,
  content  varchar(500) DEFAULT NULL,
  creationtime timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  lastmodificationtime timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(id)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4; ");

							conn.Execute(@"CREATE TABLE IF NOT EXISTS scheduler_job_history (
  batchid varchar(32) NOT NULL,
  jobid varchar(32) NOT NULL,
  status int,
  creationtime timestamp NOT NULL,
  lastmodificationtime timestamp,
  PRIMARY KEY(batchid,jobid)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4; ");
						}
						break;
					}
				default:
					{
						break;
					}
			}
		}

	}
}
