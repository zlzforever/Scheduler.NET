using Scheduler.NET.Client;
using Scheduler.NET.Common;
using System;
using System.Net.Http;

namespace Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			SchedulerNetHelper api = new SchedulerNetHelper("http://127.0.0.1:55625");
			api.CreateJob(new Job { Name = typeof(ConsoleJobProcessor).FullName, Cron = "*/1 * * * *", Group = "Test", Content = "aaa" });
			SchedulerNetClient client = new SchedulerNetClient("http://127.0.0.1:55625", "Test");
			client.Init();
			Console.Read();
		}
	}
}
