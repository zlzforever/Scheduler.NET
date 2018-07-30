using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	/// <summary>
	/// 任务接口
	/// </summary>
	public interface IJobProcessor
	{
		/// <summary>
		/// 实现任务
		/// </summary>
		/// <param name="context">任务上下文</param>
		/// <returns>是否运行成功</returns>
		bool Process(JobContext context);
	}
}
