using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.JobManager.Job
{
	public interface IJob
	{
		string Name { get; set; }

		string Cron { get; set; }

		object Data { get; set; }
	}
}
