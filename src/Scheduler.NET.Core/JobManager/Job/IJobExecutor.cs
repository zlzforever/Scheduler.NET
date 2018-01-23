using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.JobManager.Job
{
	public interface IJobExecutor<T> where T : IJob
	{
		void Execute(T job);
	}
}
