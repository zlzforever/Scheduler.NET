using Scheduler.NET.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Client
{
	public abstract class SimpleJobProcessor : IJobProcessor
	{
		public abstract bool Process(JobContext context);
	}
}
