using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core
{
	public class SchedulerException : Exception
	{
		public SchedulerException() { }

		public SchedulerException(string msg) : base(msg) { }
	}
}
