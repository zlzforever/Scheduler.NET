namespace Scheduler.NET.Common
{
	/// <summary>
	/// 任务接口
	/// </summary>
	public interface IJob
	{
		/// <summary>
		/// 任务标识
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// 任务分组
		/// </summary>
		string Group { get; set; }

		/// <summary>
		/// 任务完整的类名
		/// </summary>
		string ClassName { get; set; }

		/// <summary>
		/// 定时表达式
		/// </summary>
		string Cron { get; set; }

		/// <summary>
		/// 任务描述
		/// </summary>
		string Content { get; set; }
	}
}
