using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Scheduler.NET.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.NET.Client
{
	public abstract class KafkaJobRunner : IJobRunner
	{
		private Consumer<Null, string> _consumer;
		private readonly string _connectionString;
		private readonly string _topic;

		public KafkaJobRunner(string connectionString, string topic)
		{
			_connectionString = connectionString;
			_topic = topic;
		}

		public void Dispose()
		{
			_consumer?.Dispose();
		}

		public abstract void DoWork(string arguments);

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Start()
		{
			if (_consumer != null)
			{
				throw new SchedulerException("Runner already started.");
			}

			var config = new Dictionary<string, object>
				{
					{ "group.id", "kafka-job-runner-group" },
					{ "bootstrap.servers", _connectionString },
					{ "auto.commit.interval.ms", 5000 },
					{ "auto.offset.reset", "earliest" }
				};
			_consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8));

			_consumer.OnMessage += (_, msg)
				=>
			{
				Console.WriteLine($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");
				DoWork(msg.Value);
			};

			_consumer.OnError += (_, error)
				=> Console.WriteLine($"Error: {error}");

			_consumer.OnConsumeError += (_, msg)
				=> Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");

			_consumer.Subscribe(_topic);

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					_consumer.Poll(TimeSpan.FromMilliseconds(100));
				}
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Stop()
		{
			_consumer.Unsubscribe();
			_consumer.Dispose();
			_consumer = null;
		}
	}
}
