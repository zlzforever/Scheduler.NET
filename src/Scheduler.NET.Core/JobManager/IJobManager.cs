using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.JobManager
{
	public interface IJobManager<in T> where T : BaseJob
	{
		string Create(T job);

		void Update(T job);

		void Delete(string id);

		void Trigger(string id);
	}
}
