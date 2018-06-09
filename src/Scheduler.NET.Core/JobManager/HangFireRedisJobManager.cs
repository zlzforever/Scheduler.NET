using Microsoft.Extensions.Logging;
using Scheduler.NET.Core.JobManager.Job;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.JobManager
{
	public class HangFireRedisJobManager : HangFireJobManager<RedisJob, RedisJobExecutor>
	{
		public HangFireRedisJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
