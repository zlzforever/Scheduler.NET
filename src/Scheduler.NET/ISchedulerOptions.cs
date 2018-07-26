using System.Collections.Generic;

namespace Scheduler.NET
{
	public interface ISchedulerOptions
	{
		string HangfireStorageType { get; set; }
		string HangfireConnectionString { get; set; }
		bool UseToken { get; set; }
		HashSet<string> Tokens { get; set; }
		string TokenHeader { get; set; }
		HashSet<string> IgnoreCrons { get; set; }
	}
}
