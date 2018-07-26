using Microsoft.Extensions.Logging;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager.Job
{
	public abstract class BaseJobExecutor<T> : IJobExecutor<T> where T : IJob
	{
		/// <summary>
		/// 重试次数
		/// </summary>
		public static int RetryTimes = 5;

		protected ILogger Logger { get; }

		protected BaseJobExecutor(ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger(GetType());
		}

		public abstract void Execute(T job);
	}
}
