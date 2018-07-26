using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Common
{
	/// <inheritdoc />
	// ReSharper disable once ClassNeverInstantiated.Global
	public class CallbackJob : BaseJob
	{
		[Required]
		[Url]
		public virtual string Url { get; set; }
	}
}
