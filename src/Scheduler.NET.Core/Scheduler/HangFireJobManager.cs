using DotnetSpider.Enterprise.Core.Utils;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Core;
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
		/// <summary>
		/// 添加或者修改
		/// </summary>
		/// <param name="job"></param>
		public string AddOrUpdate(Job job)
		{
			job.Name = string.IsNullOrEmpty(job.Name) ? Guid.NewGuid().ToString("N") : job.Name;
			string json = JsonConvert.SerializeObject(job);
			RecurringJob.AddOrUpdate<JobExecutor>(job.Name, x => x.Execute(json), job.Cron);
			return job.Name;
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Remove(string jobId)
		{
			Hangfire.RecurringJob.RemoveIfExists(jobId);
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="jobId"></param>
		public void Trigger(string jobId)
		{
			RecurringJob.Trigger(jobId);
		}
	}
}
