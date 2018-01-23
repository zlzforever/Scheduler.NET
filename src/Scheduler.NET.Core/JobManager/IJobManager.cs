using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.JobManager
{
	public interface IJobManager<T> where T : IJob
	{
		string Add(T t);

		void Update(string id, T t);

		void Remove(string id);

		void Trigger(string id);
	}
}
