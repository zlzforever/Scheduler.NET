using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Scheduler.NET.Core.JobManager.Job
{
	public class RedisJobExecutor : BaseJobExecutor<RedisJob>
	{
		private static readonly Dictionary<int, ConnectionMultiplexer> RedisConnectionCaches = new Dictionary<int, ConnectionMultiplexer>();
		private static readonly object RedisConnectionCachesLocker = new object();

		public RedisJobExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override void Execute(RedisJob job)
		{
			try
			{
				var hashCode = job.ConnectString.GetHashCode();
				ConnectionMultiplexer connectionMultiplexer;
				lock (RedisConnectionCachesLocker)
				{
					if (!RedisConnectionCaches.ContainsKey(hashCode))
					{
						connectionMultiplexer = ConnectionMultiplexer.Connect(job.ConnectString);
						RedisConnectionCaches.Add(hashCode, connectionMultiplexer);
					}
					else
					{
						connectionMultiplexer = RedisConnectionCaches[hashCode];
					}
				}
				Logger.LogInformation($"Execute redis job {JsonConvert.SerializeObject(job)}.");
				Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
				{
					Logger.LogError($"Execute redis job failed [{count}] {JsonConvert.SerializeObject(job)}: {ex}.");
				}).Execute(() =>
				{
					connectionMultiplexer.GetSubscriber().Publish(job.Chanel, job.Data);
				});

			}
			catch (Exception e)
			{
				Logger.LogError($"Execute redis job {JsonConvert.SerializeObject(job)} failed: {e}.");
			}
		}
	}
}
