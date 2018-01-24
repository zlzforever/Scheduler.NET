using Jil;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.JobManager.Job
{
	public class RedisJobExecutor : BaseJobExecutor<RedisJob>
	{
		private readonly RetryPolicy _retryPolicy;
		private static Dictionary<string, ConnectionMultiplexer> _redisConnectionCaches = new Dictionary<string, ConnectionMultiplexer>();
		private static object _redisConnectionCachesLocker = new object();

		public RedisJobExecutor() : base()
		{
			_retryPolicy = Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
			{
				_logger.LogError($"Execute redis job failed [{count}]: {ex}");
			});
		}

		public override void Execute(RedisJob job)
		{
			try
			{
				lock (_redisConnectionCachesLocker)
				{
					if (!_redisConnectionCaches.ContainsKey(job.ConnectString))
					{
						_redisConnectionCaches.TryAdd(job.ConnectString, ConnectionMultiplexer.Connect(job.ConnectString));
					}
				}

				_retryPolicy.Execute(() =>
				{
					_redisConnectionCaches[job.ConnectString].GetSubscriber().Publish(job.Chanel, job.Data);
				});

				_logger.LogInformation($"Execute redis job {JSON.Serialize(job)} success.");

			}
			catch (Exception e)
			{
				_logger.LogError($"Execute redis job {JSON.Serialize(job)} failed: {e}.");
			}
		}
	}
}
