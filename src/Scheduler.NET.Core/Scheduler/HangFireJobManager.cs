using DotnetSpider.Enterprise.Core.Utils;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Core;
using Scheduler.NET.Core.Entities;
using Scheduler.NET.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Scheduler
{

	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager : IJobManager
	{

		private readonly ILogger<HangFireJobManager> _Logger;

		public HangFireJobManager(ILogger<HangFireJobManager> _logger)
		{
			this._Logger = _logger;
		}

		/// <summary>
		/// 添加、修改计划任务
		/// </summary>
		public string EnqueueHFJob(SpiderJob job)
		{
			string json = JsonConvert.SerializeObject(job);
			return BackgroundJob.Enqueue<RecurringJobService>(x => x.ExecuteJob(json));
		}

		public void QueryHFJobs()
		{
			//JobStorage.Current.GetMonitoringApi().EnqueuedJobs("", 1, 1);
			var queues = JobStorage.Current.GetMonitoringApi().Queues();
		}

		/// <summary>
		/// 添加或者修改
		/// </summary>
		/// <param name="job"></param>
		public void AddOrUpdateHFJob(SpiderJob job)
		{
			string json = JsonConvert.SerializeObject(job);
			RecurringJob.AddOrUpdate<RecurringJobService>(job.TaskId, x => x.ExecuteJob(json), job.Cron);
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

	}
}
