using System.Collections.Generic;

namespace Scheduler.NET
{
	public class SchedulerOptions : ISchedulerOptions
	{
		public const string DefaultSettingKey = "SchedulerNET";

		public bool UseToken { get; set; }
		public HashSet<string> Tokens { get; set; }
		public string HangfireStorageType { get; set; }
		public string HangfireConnectionString { get; set; }
		public string TokenHeader { get; set; }
		public HashSet<string> IgnoreCrons { get; set; }
	}
}
