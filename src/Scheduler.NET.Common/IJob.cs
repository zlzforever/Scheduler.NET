using System;

namespace Scheduler.NET.Common
{
	public interface IJob
	{
		string Id { get; set; }

		string Name { get; set; }

		string Cron { get; set; }

		string Content { get; set; }
	}
}
