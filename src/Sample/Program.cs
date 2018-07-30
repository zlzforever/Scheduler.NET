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
			SchedulerNetHelper api = new SchedulerNetHelper("http://localhost:5001");
			api.CreateJob(new Job { ClassName = typeof(ConsoleJobProcessor).FullName, Cron = "*/1 * * * *", Group = "Test", Content = "aaa" });
			api.CreateCallbackJob(new CallbackJob { ClassName = "", Content = "", Cron = "*/1 * * * *", Group = "group1", Method = HttpMethod.Get, Url = "http://www.baidu.com" });
			SchedulerNetClient client = new SchedulerNetClient("Test", "http://localhost:5001");
			client.Init();
			Console.Read();
		}
	}
}
