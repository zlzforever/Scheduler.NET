using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.NET.Core.JobManager.Job
{
	public abstract class BaseJobExecutor<T> : IJobExecutor<T> where T : IJob
	{
		protected readonly ILogger Logger;

		/// <summary>
		/// 重试次数
		/// </summary>
		public static int RetryTimes = 5;

		protected BaseJobExecutor()
		{
			Logger = SchedulerExtensions.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
		}

		public abstract void Execute(T job);
	}
}
