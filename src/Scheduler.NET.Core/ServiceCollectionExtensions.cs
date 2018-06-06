using DotnetSpider.Enterprise.Core.JobManager;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

		public static void AddScheduler(this IServiceCollection services, IMvcBuilder builder, IConfiguration configuration)
		{
			services.AddHttpClient();

			builder.AddMvcOptions(options => options.Filters.Add<HttpGlobalExceptionFilter>());

			var section = configuration.GetSection(SchedulerConfiguration.DefaultSettingKey);

			services.Configure<SchedulerConfiguration>(section);

			services.AddTransient<IJobManager<CallbackJob>, HangFireCallbackJobManager>();
			services.AddTransient<ISchedulerConfiguration, SchedulerConfiguration>();
			services.AddTransient<IJobManager<RedisJob>, HangFireRedisJobManager>();

			var schedulerConfig = section.Get<SchedulerConfiguration>();

			switch (schedulerConfig.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						services.AddHangfire(r => r.UseSqlServerStorage(schedulerConfig.HangfireConnectionString));
						break;
					}
				case "redis":
					{
						services.AddHangfire(r => r.UseRedisStorage(schedulerConfig.HangfireConnectionString));
						break;
					}
			}
		}

	}
}
