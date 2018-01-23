using DotnetSpider.Enterprise.Core.JobManager;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Scheduler.NET.Core.Filter;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core
{
	public static class SchedulerExtensions
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
			var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
			loggerFactory.AddConsole(app.ApplicationServices.GetRequiredService<IConfiguration>().GetSection("Logging"));
			loggerFactory.AddDebug();
			loggerFactory.AddNLog();
		}

		public static void AddScheduler(this IServiceCollection services, IMvcBuilder mvcBuilder, IConfiguration configuration)
		{
			mvcBuilder.AddMvcOptions(options => options.Filters.Add<HttpGlobalExceptionFilter>());

			var section = configuration.GetSection(SchedulerConfiguration.DefaultSettingKey);

			services.Configure<SchedulerConfiguration>(section);

			services.AddTransient<IJobManager<CallbackJob>, HangFireCallbackJobManager>();
			services.AddTransient<ISchedulerConfiguration, SchedulerConfiguration>();

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
