using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.JobManager
{
	public interface IJobManager<T> where T : IJob
	{
		string Create(T job);

		void Update(T job);

		void Delete(string id);

		void Trigger(string id);
	}
}
