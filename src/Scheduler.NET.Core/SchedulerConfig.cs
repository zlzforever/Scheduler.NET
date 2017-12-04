namespace Scheduler.NET.Core
{
	public class SchedulerConfig : ISchedulerConfig
	{
		public string HangfireStorageType { get; set; }
		public string HangfireConnectionString { get; set; }
		public string Host { get; set; }
	}
}
