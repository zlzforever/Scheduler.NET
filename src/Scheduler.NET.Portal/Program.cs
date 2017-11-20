using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;
using System.Runtime.InteropServices;

namespace Scheduler.NET.Portal
{
	public class Program
	{
		private static readonly string Url;

		static Program()
		{
			string hostUrl = "http://*:5001";
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (File.Exists(Path.Combine(AppContext.BaseDirectory, "host.config")))
				{
					hostUrl = File.ReadAllLines("host.config")[0];
				}
			}
			Url = hostUrl;
		}

		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>().UseUrls(Url)
				.Build();
	}
}
