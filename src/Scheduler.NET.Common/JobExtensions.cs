using System;

namespace Scheduler.NET.Common
{
	public static class JobExtensions
	{
		public static JobContext ToContext(this IJob job)
		{
			return new JobContext
			{
				Id = job.Id,
				ClassName = job.ClassName,
				Content = job.Content,
				Cron = job.Cron,
				FireTime = DateTime.Now,
				Group = job.Group
			};
		}
	}
}
