namespace Scheduler.NET.Common
{
	public class StandardResult
	{
		public int Code { get; set; }
		public Status Status { get; set; }
		public string Message { get; set; }
		public dynamic Data { get; set; }
	}
}
