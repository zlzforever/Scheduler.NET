namespace Scheduler.NET.Core.JobManager.Job
{
	public interface IJob
	{
		string Name { get; set; }

		string Cron { get; set; }

		string Data { get; set; }
	}
}
