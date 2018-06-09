using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Core.JobManager.Job
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
