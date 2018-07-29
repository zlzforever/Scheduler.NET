namespace Scheduler.NET.Common
{
	public class StandardResult
	{
		public int Code { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public dynamic Data { get; set; }
	}
}
