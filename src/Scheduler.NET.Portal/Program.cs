using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Scheduler.NET.Portal
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configurationFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
							"appsettings.Development.json" : "appsettings.json";

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "{Date}.log"))
				.WriteTo.Console()
				.CreateLogger();

			Log.Information("Welcome to Scheduler.Net!");

			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile(configurationFile, optional: true)
				.Build();

			var host = WebHost.CreateDefaultBuilder(args).UseConfiguration(config)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>().UseSerilog().Build();
			host.Run();
		}
	}
}
