using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager.Job
{
	public class KafkaJobExecutor : BaseJobExecutor<KafkaJob>
	{
		private static readonly HashObjectPool<Producer<Null, string>> _pool;

		static KafkaJobExecutor()
		{
			_pool = new HashObjectPool<Producer<Null, string>>((item) =>
			{
				var config = new Dictionary<string, object>
				{
					{ "bootstrap.servers", item.Key },
					{"socket.blocking.max.ms", 1}
				};
				return new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
			});
		}

		public KafkaJobExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override void Execute(KafkaJob job)
		{
			try
			{
				Producer<Null, string> producer = _pool.Get(new ObjectKey(job.ConnectString));

				Logger.LogInformation($"Execute kafka job {JsonConvert.SerializeObject(job)}.");
				Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
				{
					Logger.LogError($"Execute kafka job failed [{count}] {JsonConvert.SerializeObject(job)}: {ex}.");
				}).Execute(() =>
				{
					var dr = producer.ProduceAsync(job.Topic, null, job.Content).Result;
					Logger.LogInformation($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
				});
			}
			catch (Exception e)
			{
				Logger.LogError($"Execute kafka job {JsonConvert.SerializeObject(job)} failed: {e}.");
			}
		}
	}
}
