using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Scheduler.NET.Core.Scheduler
{
	[DataContract()]
	public class SpiderJob
	{
		[DataMember(Name = "taskId")]
		public String TaskId { get; set; }

		[DataMember(Name = "cron")]
		public String Cron { get; set; }
	}

}
