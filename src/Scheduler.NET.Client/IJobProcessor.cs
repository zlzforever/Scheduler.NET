using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Client
{
	public interface IJobProcessor : IDisposable
	{
		void Process(string arguments);
		void Start();
	}
}
