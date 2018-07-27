using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Polly;
using StackExchange.Redis;
using System;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager.Job
{
	public class RedisJobExecutor : BaseJobExecutor<RedisJob>
	{
		private static readonly HashObjectPool<ConnectionMultiplexer> _pool;

		static RedisJobExecutor()
		{
			_pool = new HashObjectPool< ConnectionMultiplexer>((item) =>
			{
				return ConnectionMultiplexer.Connect(item.Key.ToString());
			});
		}

		public RedisJobExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override void Execute(RedisJob job)
		{
			try
			{
				ConnectionMultiplexer connectionMultiplexer = _pool.Get(new ObjectKey(job.ConnectString));

				Logger.LogInformation($"Execute redis job {JsonConvert.SerializeObject(job)}.");
				Policy.Handle<Exception>().Retry(RetryTimes, (ex, count) =>
				{
					Logger.LogError($"Execute redis job failed [{count}] {JsonConvert.SerializeObject(job)}: {ex}.");
				}).Execute(() =>
				{
					connectionMultiplexer.GetSubscriber().Publish(job.Channel, job.Detail);
				});

			}
			catch (Exception e)
			{
				Logger.LogError($"Execute redis job {JsonConvert.SerializeObject(job)} failed: {e}.");
			}
		}
	}
}
