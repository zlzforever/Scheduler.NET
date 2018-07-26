using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Common
{
	public class RedisJob : BaseJob
	{
		[Required]
		[StringLength(200)]
		public virtual string ConnectString { get; set; }

		[Required]
		[StringLength(100)]
		public virtual string Channel { get; set; }
	}
}
