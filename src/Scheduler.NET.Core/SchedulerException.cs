using System;

namespace Scheduler.NET.Core
{
	public class SchedulerException : Exception
	{
		public SchedulerException() { }

		public SchedulerException(string msg) : base(msg) { }
	}
}
