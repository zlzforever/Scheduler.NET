using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scheduler.NET.Core;

namespace Scheduler.NET.Portal
{
	public class Program
	{
		public static void Main(string[] args)
		{
#if DEBUG
			var appsetingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json");
#else
			var appsetingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
#endif
			var appsetings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(appsetingsPath)).SelectToken($"$.{SchedulerConfiguration.DefaultSettingKey}").ToObject<SchedulerConfiguration>();

			var url = "http://*:5001";
			if (!string.IsNullOrEmpty(appsetings.Host) && !string.IsNullOrWhiteSpace(appsetings.Host))
			{
				url = appsetings.Host;
			}

			IWebHost host = WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>().UseUrls(url)
				.Build();
			host.Run();
		}
	}
}
