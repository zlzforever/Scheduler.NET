using Scheduler.NET.Client;
using Scheduler.NET.Common;
using System;

namespace Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			SchedulerNetHelper api = new SchedulerNetHelper("http://localhost:5001");
			api.CreateJob(new Job { ClassName = typeof(ConsoleJobProcessor).FullName, Cron = "*/1 * * * *", Group = "Test", Content = "aaa" });
			SchedulerNetClient client = new SchedulerNetClient("Test", "http://localhost:5001");
			client.Init();
			Console.Read();
		}
	}
}
