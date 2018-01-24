using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.NET.Core.JobManager.Job
{
	public abstract class BaseJobExecutor<T> : IJobExecutor<T> where T : IJob
	{
		protected readonly ILogger _logger;

		public BaseJobExecutor()
		{
			_logger = SchedulerExtensions.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
		}

		public abstract void Execute(T job);
	}
}
