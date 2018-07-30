using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;
using Dapper;

namespace Scheduler.NET.JobManager
{
	/// <summary>
	/// Http 回调任务管理器
	/// </summary>
	public class HangFireCallbackJobManager : HangFireJobManager<CallbackJob, CallbackJobExecutor>
	{
		public HangFireCallbackJobManager(ILoggerFactory loggerFactory, ISchedulerOptions options) : base(loggerFactory, options)
		{
		}

		protected override void InsertSchedulerJob(CallbackJob job)
		{
			using (var conn = _options.CreateConnection())
			{
				var currentDatetimeSql = _options.GetCurrentDatetimeSql();
				conn?.Execute(
					$"INSERT INTO scheduler_job(id,{_options.GetGroupSql()},name,cron,content,jobtype,url,method,creationtime,lastmodificationtime) values (@Id,@Group,@Name,@Cron,@Content,'callbackjob',@Url,@Method,{currentDatetimeSql},{currentDatetimeSql})",
					job);
			}
		}

		protected override void UpdateSchedulerJob(CallbackJob job)
		{
			using (var conn = _options.CreateConnection())
			{
				conn?.Execute(
					$"UPDATE job SET {_options.GetGroupSql()}=@Group,name=@Name,cron=@Cron,content=@Content,url=@Url,method=@Method,lastmodificationtime={_options.GetCurrentDatetimeSql()} WHERE id=@Id",
					job);
			}
		}
	}
}
