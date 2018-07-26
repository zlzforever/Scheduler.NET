using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager;
using Scheduler.NET.Common;

namespace Scheduler.NET.Controllers.V1
{
	[Route("api/v1.0/[controller]")]
	public class KafkaJobController : BaseController<KafkaJob>
	{
		public KafkaJobController(IJobManager<KafkaJob> jobManager, ILoggerFactory loggerFactory, ISchedulerOptions configuration) : base(jobManager, loggerFactory, configuration)
		{
		}
	}
}
