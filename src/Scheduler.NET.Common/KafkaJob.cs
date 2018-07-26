using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Scheduler.NET.Common
{
	public class KafkaJob : BaseJob
	{
		[Required]
		[StringLength(200)]
		public virtual string ConnectString { get; set; }

		[Required]
		[StringLength(100)]
		public virtual string Topic { get; set; }
	}
}
