namespace Scheduler.NET.Core
{
	public interface ISchedulerConfig
	{
		string HangfireStorageType { get; set; }
		string HangfireConnectionString { get; set; }
		string Host { get; set; }
	}
}
