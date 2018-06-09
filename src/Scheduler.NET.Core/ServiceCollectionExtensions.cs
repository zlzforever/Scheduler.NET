using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.NET.Core.Filter;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;
using System;

namespace Scheduler.NET.Core
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 必须放在UseMvc前面
		/// </summary>
		/// <param name="app"></param>
		public static void UseScheduler(this IApplicationBuilder app)
		{
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

			builder.AddMvcOptions(options => options.Filters.Add<HttpGlobalExceptionFilter>());

			builder.Services.AddSingleton<ISchedulerOptions>(schedulerOptions);

			builder.Services.AddTransient<IJobManager<CallbackJob>, HangFireCallbackJobManager>();
			builder.Services.AddTransient<IJobManager<RedisJob>, HangFireRedisJobManager>();

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
			}

			return builder;
		}

		public static IMvcBuilder AddScheduler(this IMvcBuilder builder, IConfiguration configuration)
		{
			var section = configuration.GetSection(SchedulerOptions.DefaultSettingKey);
			var schedulerOptions = section.Get<SchedulerOptions>();

			return builder.AddScheduler(options =>
			{
				options.HangfireConnectionString=schedulerOptions.HangfireConnectionString;
				options.HangfireStorageType = schedulerOptions.HangfireStorageType;
				options.IgnoreCrons = schedulerOptions.IgnoreCrons;
				options.TokenHeader = schedulerOptions.TokenHeader;
				options.Tokens = schedulerOptions.Tokens;
				options.UseToken = schedulerOptions.UseToken;
			});
		}

	}
}
