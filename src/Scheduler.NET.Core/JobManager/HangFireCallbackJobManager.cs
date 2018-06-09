using Microsoft.Extensions.Logging;
using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.JobManager
{
	/// <summary>
	/// Http 回调任务管理器
	/// </summary>
	public class HangFireCallbackJobManager : HangFireJobManager<CallbackJob, CallbackJobExecutor>
	{
		public HangFireCallbackJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
