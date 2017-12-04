using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.NET.Core;

namespace Scheduler.NET.Portal
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IHostingEnvironment env,IConfiguration configuration)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath);


			if (env.IsDevelopment())
			{
				builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
				builder.AddUserSecrets<Startup>();
			}
			else
			{
				builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			}
			builder.AddEnvironmentVariables();
			_configuration = builder.Build();
		}
 
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddScheduler(_configuration);
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

			app.UseScheduler();

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
