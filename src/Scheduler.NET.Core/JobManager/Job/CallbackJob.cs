using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Core.JobManager.Job
{
	public class CallbackJob : BaseJob
	{
		[Required]
		[Url]
		public virtual string Url { get; set; }
	}
}
