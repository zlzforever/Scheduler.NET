using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using System;
using System.Collections.Generic;
using System.Text;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager
{
	public class HangFireRedisJobManager : HangFireJobManager<RedisJob, RedisJobExecutor>
	{
		public HangFireRedisJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
