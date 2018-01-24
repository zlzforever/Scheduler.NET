using Jil;
using Microsoft.Extensions.Logging;
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
		private static readonly Dictionary<string, ConnectionMultiplexer> RedisConnectionCaches = new Dictionary<string, ConnectionMultiplexer>();
		private static readonly object RedisConnectionCachesLocker = new object();

		public RedisJobExecutor() : base()
		{
			_retryPolicy = Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
			{
				Logger.LogError($"Execute redis job failed [{count}]: {ex}");
			});
		}

		public override void Execute(RedisJob job)
		{
			try
			{
				lock (RedisConnectionCachesLocker)
				{
					if (!RedisConnectionCaches.ContainsKey(job.ConnectString))
					{
						RedisConnectionCaches.TryAdd(job.ConnectString, ConnectionMultiplexer.Connect(job.ConnectString));
					}

					_retryPolicy.Execute(() =>
					{
						RedisConnectionCaches[job.ConnectString].GetSubscriber().Publish(job.Chanel, job.Data);
					});
				}

				Logger.LogInformation($"Execute redis job {JSON.Serialize(job)} success.");

			}
			catch (Exception e)
			{
				Logger.LogError($"Execute redis job {JSON.Serialize(job)} failed: {e}.");
			}
		}
	}
}
