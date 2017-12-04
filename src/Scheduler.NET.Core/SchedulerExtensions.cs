using DotnetSpider.Enterprise.Core.Scheduler;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.NET.Core.Filter;
using Scheduler.NET.Core.Scheduler;

namespace Scheduler.NET.Core
{
	public static class SchedulerExtensions
	{
		public static void UseScheduler(this IApplicationBuilder app)
		{
			app.UseHangfireServer();
			app.UseHangfireDashboard("/hangfire", new DashboardOptions()
			{
				Authorization = new[] { new CustomAuthorizeFilter() }
			});
		}

		public static void AddScheduler(this IServiceCollection services, IConfiguration configuration)
		{
			var section = configuration.GetSection("AppSettings");

			services.Configure<SchedulerConfig>(section);

			services.AddTransient<IJobManager, HangFireJobManager>();
			var schedulerConfig = section.Get<SchedulerConfig>();

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
