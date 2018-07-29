using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	public interface IJobProcessor
	{
		bool Process(JobContext context);
	}
}
