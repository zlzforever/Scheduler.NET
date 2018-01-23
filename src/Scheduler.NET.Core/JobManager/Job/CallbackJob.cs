using Jil;
using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Core.JobManager.Job
{
	public class CallbackJob : BaseJob
	{
		[Required]
		[StringLength(200)]
		public virtual string Url { get; set; }
	}
}
