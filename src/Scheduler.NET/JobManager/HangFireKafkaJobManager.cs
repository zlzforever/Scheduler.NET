using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager
{
	public class HangFireKafkaJobManager : HangFireJobManager<KafkaJob, KafkaJobExecutor>
	{
		public HangFireKafkaJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
