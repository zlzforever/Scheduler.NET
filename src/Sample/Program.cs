using Scheduler.NET.Client;
using Scheduler.NET.Common;
using System;

namespace Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new SchedulerNetClient("http://localhost:5001");
			var topic = "mytopic";
			var id = client.Create(new KafkaJob { ConnectString = "192.168.90.106:9092", Detail = "", Cron = "*/2 * * * *", Name = "test", Topic = topic });
			//client.Update(new KafkaJob { Id = id, ConnectString = "192.168.90.106:9092", Detail = "", Cron = "*/1 * * * *", Name = "test", Topic = topic });
			MyKafkaJobRunner runner = new MyKafkaJobRunner("192.168.90.106:9092", topic);
			runner.Start();
			Console.Read();
		}

		public class MyKafkaJobRunner : KafkaJobProcessor
		{
			public MyKafkaJobRunner(string connectionString, string topic) : base(connectionString, topic)
			{
			}

			public override void Process(string arguments)
			{
				Console.WriteLine(arguments);
			}
		}
	}
}
