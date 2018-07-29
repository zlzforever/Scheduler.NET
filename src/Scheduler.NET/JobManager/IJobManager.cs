using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager
{
	public interface IJobManager<in T> where T : IJob
	{
		string Create(T job);

		void Update(T job);

		void Delete(string id);

		void Fire(string id);
	}
}
