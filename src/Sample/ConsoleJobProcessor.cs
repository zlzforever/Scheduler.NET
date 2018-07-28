using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Scheduler.NET.Client;
using Scheduler.NET.Common;

namespace Sample
{
	public class ConsoleJobProcessor : SimpleJobProcessor
	{
		public override bool Process(JobContext context)
		{
			Console.WriteLine(JsonConvert.SerializeObject(context));
			return true;
		}
	}
}
