using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Core.Scheduler
{
	public class Job
	{
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		[Required]
		[StringLength(20)]
		public virtual string Cron { get; set; }

		[Required]
		[StringLength(200)]
		public virtual string Url { get; set; }

		public virtual string Data { get; set; }
	}
}
