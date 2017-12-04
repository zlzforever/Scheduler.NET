namespace Scheduler.NET.Core.Scheduler
{
	public interface IJobManager
	{
		string AddOrUpdate(Job job);

		void Remove(string jobId);

		void Trigger(string jobId);
	}
}
