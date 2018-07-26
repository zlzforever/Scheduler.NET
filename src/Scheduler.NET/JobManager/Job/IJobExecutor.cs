using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager.Job
{
	public interface IJobExecutor<in T> where T : IJob
	{
		void Execute(T job);
	}
}
