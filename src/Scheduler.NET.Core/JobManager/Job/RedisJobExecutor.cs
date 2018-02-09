using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Scheduler.NET.Core.JobManager.Job
{
	public class RedisJobExecutor : BaseJobExecutor<RedisJob>
	{
		private readonly RetryPolicy _retryPolicy;
		private static readonly Dictionary<int, ConnectionMultiplexer> RedisConnectionCaches = new Dictionary<int, ConnectionMultiplexer>();
		private static readonly object RedisConnectionCachesLocker = new object();

		public RedisJobExecutor() : base()
		{
			_retryPolicy = Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
			{
				Logger.LogError($"Execute redis job failed [{count}]: {ex}.");
			});
		}

		public override void Execute(RedisJob job)
		{
			try
			{
				var hashCode = job.ConnectString.GetHashCode();
				lock (RedisConnectionCachesLocker)
				{
					if (!RedisConnectionCaches.ContainsKey(hashCode))
					{
						RedisConnectionCaches.TryAdd(hashCode, ConnectionMultiplexer.Connect(job.ConnectString));
					}
				}
				_retryPolicy.Execute(() =>
				{
					RedisConnectionCaches[hashCode].GetSubscriber().Publish(job.Chanel, job.Data);
				});
				Logger.LogInformation($"Execute redis job {JsonConvert.SerializeObject(job)} success.");
			}
			catch (Exception e)
			{
				Logger.LogError($"Execute redis job {JsonConvert.SerializeObject(job)} failed: {e}.");
			}
		}
	}
}
