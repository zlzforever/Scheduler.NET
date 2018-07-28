using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Common
{
	public class JobContext : Job
	{
		public DateTime FireTime { get; set; }
	}
}
