using Hangfire;
using Hangfire.SqlServer;
using Scheduler.NET.Core;
using Scheduler.NET.Core.Domain;
using Scheduler.NET.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Scheduler
{

	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager : IJobManager
	{

		static HangFireJobManager()
		{
			//InitHangFire();
		}

		private static void InitHangFire()
		{
			var options = new SqlServerStorageOptions
			{
				PrepareSchemaIfNecessary = true
			};
			GlobalConfiguration.Configuration.UseSqlServerStorage(SchedulerConfig.SqlServerConnectString, options);
		}

		/// <summary>
		/// 添加、修改计划任务
		/// </summary>
		public static string EnqueueHFJob(SpiderJob job)
		{
			var id = BackgroundJob.Enqueue(() => Method(job.TaskId));
			return id;
		}

		/// <summary>
		/// 添加或者修改
		/// </summary>
		/// <param name="job"></param>
		public static void AddOrUpdateHFJob(SpiderJob job)
		{
			RecurringJob.AddOrUpdate(job.TaskId, () => Method(job.TaskId), job.Cron);
		}
		
		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void RemoveHFJob(String jobId)
		{
			RecurringJob.RemoveIfExists(jobId);
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="jobId"></param>
		public void Trigger(String jobId)
		{
			RecurringJob.Trigger(jobId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_param"></param>
		public static void Method(String _param)
		{
			//invoke api
		}
	}
}
