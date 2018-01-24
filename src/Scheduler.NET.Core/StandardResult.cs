namespace Scheduler.NET.Core
{
	public class StandardResult
	{
		public int Code { get; set; }
		public Status Status { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
	}
}
