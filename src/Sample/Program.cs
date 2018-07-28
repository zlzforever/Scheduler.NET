using Scheduler.NET.Client;
using Scheduler.NET.Common;
using System;

namespace Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			SchedulerNETHelper api = new SchedulerNETHelper("http://localhost:5001");
			api.Create(new Job { ClassName = typeof(ConsoleJobProcessor).FullName, Cron = "*/1 * * * *", Group = "Test", Content = "aaa" });
			SchedulerNETClient client = new SchedulerNETClient("Test", "http://localhost:5001");
			client.Init();
			Console.Read();
		}
	}
}
