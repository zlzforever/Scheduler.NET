using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Common
{
	public class JobContext
	{
		public IJob Job { get; set; }
		public int RetryCount { get; set; }
	}
}
