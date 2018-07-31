using System.Collections.Generic;

namespace Scheduler.NET
{
	public interface ISchedulerOptions
	{
		HangfirefireOptions Hangfire { get; set; }
		StorageType StorageType { get; set; }
		string ConnectionString { get; set; }
		bool UseToken { get; set; }
		HashSet<string> Tokens { get; set; }
		string TokenHeader { get; set; }
		HashSet<string> IgnoreCrons { get; set; }
		CacheOptions Cache { get; set; }
	}
}
