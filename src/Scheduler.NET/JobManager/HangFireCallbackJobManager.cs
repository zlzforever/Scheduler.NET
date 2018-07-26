using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager
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
