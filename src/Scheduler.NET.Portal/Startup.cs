using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Hangfire;
using Scheduler.NET.Core;
using Microsoft.Extensions.Options;
using System.IO;
using DotnetSpider.Enterprise.Core.Scheduler;
using Scheduler.NET.Core.Scheduler;

namespace Scheduler.NET.Portal
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			if (env.IsDevelopment())
			{
				builder.AddUserSecrets<Startup>();
			}
			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public SchedulerConfig _SchedulerConfig { get; set; }
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.Configure<SchedulerConfig>(this.Configuration.GetSection("AppSettings"));
			
			services.AddTransient<IJobManager,HangFireJobManager >();
			
			_SchedulerConfig = services.BuildServiceProvider().GetService<IOptions<SchedulerConfig>>().Value;
			if (_SchedulerConfig.SqlConfig.Enable)
			{
				services.AddHangfire(r => r.UseSqlServerStorage(_SchedulerConfig.SqlConfig.ConnectionString));
			}
			else
			{
				services.AddHangfire(x =>
				{
					x.UseRedisStorage(_SchedulerConfig.RedisConfig.ConnectionString);
				});
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseHangfireServer();
			app.UseHangfireDashboard();

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
