using System;

namespace Scheduler.NET.Common
{
	public class SchedulerNetException : Exception
	{
		public SchedulerNetException() { }

		public SchedulerNetException(string msg, Exception inner = null) : base(msg, inner) { }
	}
}
