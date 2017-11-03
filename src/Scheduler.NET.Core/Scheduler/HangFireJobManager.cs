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

		public static void AddOrUpdateHFJob(SpiderJob job)
		{
			RecurringJob.AddOrUpdate(() => Method(job.TaskId), job.Cron);
			//RecurringJob.AddOrUpdate(() => Console.WriteLine("1"), job.Cron);
		}

		public static void AddHFJob(String jobId, SpiderJob job)
		{
			RecurringJob.AddOrUpdate(jobId, () => Method(job.TaskId), job.Cron);
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void RemoveHFJob(String jobId)
		{
			RecurringJob.RemoveIfExists(jobId);
		}

		public static void Method(String _param)
		{

		}
	}
}
