namespace Scheduler.NET.Core.JobManager.Job
{
	public interface IJobExecutor<in T> where T : IJob
	{
		void Execute(T job);
	}
}
