using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager
{
	public interface IJobManager<in T> where T : Common.Job
	{
		string Create(T job);

		void Update(T job);

		void Delete(string id);

		void Trigger(string id);
	}
}
