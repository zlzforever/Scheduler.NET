using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.NET.Filter;
using Scheduler.NET.JobManager;
using Scheduler.NET.Common;
using System;
using Hangfire.MySql.Core;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MySql.Data.MySqlClient;
using Hangfire.SqlServer;
using Hangfire.Redis;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Hangfire.MemoryStorage;

namespace Scheduler.NET
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 必须放在UseMvc前面
		/// </summary>
		/// <param name="app"></param>
		public static void UseSchedulerNet(this IApplicationBuilder app)
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

		public static IMvcBuilder AddSchedulerNet(this IMvcBuilder builder, Action<ISchedulerOptions> configOptions)
		{
			ISchedulerOptions options = new SchedulerOptions();
			configOptions(options);

			builder.Services.AddHttpClient();
			builder.AddMvcOptions(o => o.Filters.Add<HttpGlobalExceptionFilter>());

			builder.Services.AddSingleton(options);
			builder.Services.AddTransient<IJobManager<CallbackJob>, HangFireCallbackJobManager>();
			builder.Services.AddTransient<IJobManager<Job>, HangFireJobManager>();

			var signalRbuilder = builder.Services.AddSignalR();

			switch (options.Cache.Type)
			{
				case StorageType.Redis:
					{
						builder.Services.AddSingleton<ISchedulerNetCache, RedisSchedulerNetCache>();
						signalRbuilder.AddRedis(options.Cache.ConnectionString);
						break;
					}
				case StorageType.Memory:
					{
						builder.Services.AddSingleton<ISchedulerNetCache, MemorySchedulerNetCache>();
						break;
					}
				default:
					{
						throw new NotImplementedException($"{options.Cache.Type} cache");
					}
			}

			builder.Services.AddHangfire(config => { });

			switch (options.Hangfire.StorageType)
			{
				case StorageType.Memory:
					{
						GlobalConfiguration.Configuration.UseStorage(new MemoryStorage());
						break;
					}
				case StorageType.SqlServer:
					{
						GlobalConfiguration.Configuration.UseStorage(new SqlServerStorage(options.Hangfire.ConnectionString));
						break;
					}
				case StorageType.Redis:
					{
						GlobalConfiguration.Configuration.UseStorage(new RedisStorage(options.Hangfire.ConnectionString));
						break;
					}
				case StorageType.MySql:
					{
						GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(options.Hangfire.ConnectionString));
						break;
					}
			}

			InitSchedulerNetDatabase(options);
			return builder;
		}

		public static IMvcBuilder AddSchedulerNet(this IMvcBuilder builder, IConfiguration configuration)
		{
			var section = configuration.GetSection(SchedulerOptions.SectionName);
			var schedulerOptions = section.Get<SchedulerOptions>();

			return builder.AddSchedulerNet(options =>
			{
				options.Cache = schedulerOptions.Cache;
				options.ConnectionString = schedulerOptions.ConnectionString;
				options.Hangfire = schedulerOptions.Hangfire;
				options.IgnoreCrons = schedulerOptions.IgnoreCrons;
				options.StorageType = schedulerOptions.StorageType;
				options.TokenHeader = schedulerOptions.TokenHeader;
				options.Tokens = schedulerOptions.Tokens;
				options.UseToken = schedulerOptions.UseToken;
			});
		}

		private static void InitSchedulerNetDatabase(ISchedulerOptions schedulerOptions)
		{
			switch (schedulerOptions.StorageType)
			{
				case StorageType.SqlServer:
					{
						using (var conn = schedulerOptions.CreateConnection())
						{
							conn.Execute(@"
IF NOT EXISTS  (SELECT  * FROM dbo.SysObjects WHERE ID = object_id(N'[scheduler_job]') AND OBJECTPROPERTY(ID, 'IsTable') = 1)
CREATE TABLE scheduler_job (
  id varchar(32) NOT NULL,
  [group] varchar(255) NOT NULL,
  name  varchar(255) NOT NULL,
  cron  varchar(50) NOT NULL,
  content  varchar(500) DEFAULT NULL,
  jobtype varchar(20) NOT NULL,
  url  varchar(500) DEFAULT NULL,
  method  varchar(20) DEFAULT NULL,
  isenable bit,
  status int,
  creationtime DateTime NOT NULL,
  lastmodificationtime DateTime,
  PRIMARY KEY(id)
)");

							conn.Execute(@"
IF NOT EXISTS  (SELECT  * FROM dbo.SysObjects WHERE ID = object_id(N'[scheduler_job_history]') AND OBJECTPROPERTY(ID, 'IsTable') = 1)
CREATE TABLE scheduler_job_history (
  batchid varchar(32) NOT NULL,
  jobid varchar(32) NOT NULL,
  clientip varchar(20) NOT NULL,
  connectionid varchar(32) NOT NULL,
  status int,
  creationtime DateTime NOT NULL,
  lastmodificationtime DateTime,
  PRIMARY KEY(batchid,jobid,clientip,connectionid)
)");
						}

						break;
					}
				case StorageType.MySql:
					{
						using (var conn = schedulerOptions.CreateConnection())
						{
							conn.Execute(@"
CREATE TABLE IF NOT EXISTS scheduler_job (
  id varchar(32) NOT NULL,
  `group` varchar(255) NOT NULL,
  name  varchar(255) NOT NULL,
  cron  varchar(50) NOT NULL,
  content  varchar(500) DEFAULT NULL,
  jobtype varchar(20) NOT NULL,
  url  varchar(500) DEFAULT NULL,
  method  varchar(20) DEFAULT NULL,
  isenable tinyint(1),
  creationtime timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  lastmodificationtime timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(id)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4; ");

							conn.Execute(@"
CREATE TABLE IF NOT EXISTS scheduler_job_history (
  batchid varchar(32) NOT NULL,
  jobid varchar(32) NOT NULL,
  clientip varchar(20) NOT NULL,
  connectionid varchar(32) NOT NULL,
  status int,
  creationtime timestamp NOT NULL,
  lastmodificationtime timestamp,
  PRIMARY KEY(batchid,jobid,clientip,connectionid)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4; ");
						}
						break;
					}
			}
		}
	}
}
