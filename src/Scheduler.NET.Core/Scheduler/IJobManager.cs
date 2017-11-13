using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.Scheduler
{
	public interface IJobManager
	{
		void AddOrUpdateHFJob(SpiderJob job);

		string EnqueueHFJob(SpiderJob job);

		void RemoveHFJob(String jobId);

		void Trigger(String jobId);

	}
}
