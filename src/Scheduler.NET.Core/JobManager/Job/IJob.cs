namespace Scheduler.NET.Core.JobManager.Job
{
	public interface IJob
	{
		string Id { get; set; }

		string Name { get; set; }

		string Cron { get; set; }

		string Data { get; set; }
	}
}
