using Scheduler.NET.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Scheduler.NET.Client
{
	public abstract class RedisJobProcessor : IJobProcessor
	{
		private static readonly HashObjectPool<ISubscriber> _pool;
		private readonly string _connectionString;
		private readonly string _channel;
		private ISubscriber _subscriber;

		static RedisJobProcessor()
		{
			_pool = new HashObjectPool<ISubscriber>((item) =>
			{
				var conn = ConnectionMultiplexer.Connect(item.Key.ToString());
				return conn.GetSubscriber();
			});
		}

		public RedisJobProcessor(string connectionString, string channel)
		{
			_connectionString = connectionString;
			_channel = channel;
		}

		public void Dispose()
		{
		}

		public abstract void Process(string arguments);

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Start()
		{
			if (_subscriber != null)
			{
				throw new SchedulerException("Runner already started.");
			}
			_subscriber = _pool.Get(new ObjectKey(_connectionString));
			_subscriber.Subscribe(_channel, (c, v) =>
			{
				Process(v);
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Stop()
		{
			_subscriber.Unsubscribe(_channel);
			_subscriber = null;
		}
	}
}
