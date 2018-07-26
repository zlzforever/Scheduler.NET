using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Client
{
	public interface IJobRunner : IDisposable
	{
		void DoWork(string arguments);
		void Start();
	}
}
