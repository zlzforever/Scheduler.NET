using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core.Scheduler
{
    public interface IJob
    {
		void Run();
    }
}
