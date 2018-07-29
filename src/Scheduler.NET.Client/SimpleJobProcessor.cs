using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	public abstract class SimpleJobProcessor : IJobProcessor
	{
		public abstract bool Process(JobContext context);
	}
}
