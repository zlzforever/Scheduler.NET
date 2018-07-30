using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Scheduler.NET")]
namespace Scheduler.NET.Common
{
	internal static class JobExtensions
	{
		public static JobContext ToContext(this IJob job)
		{
			return new JobContext
			{
				Id = job.Id,
				Name = job.Name,
				Content = job.Content,
				Cron = job.Cron,
				FireTime = DateTime.Now,
				Group = job.Group
			};
		}
	}
}
