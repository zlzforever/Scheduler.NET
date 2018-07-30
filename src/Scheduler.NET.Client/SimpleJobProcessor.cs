using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	/// <summary>
	/// 简单任务实现
	/// </summary>
	public abstract class SimpleJobProcessor : IJobProcessor
	{
		/// <summary>
		/// 实现任务
		/// </summary>
		/// <param name="context">任务上下文</param>
		/// <returns>是否运行成功</returns>
		public abstract bool Process(JobContext context);
	}
}
