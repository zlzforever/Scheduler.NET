using Hangfire;
using Newtonsoft.Json;
using NLog;
using Scheduler.NET.Core.Scheduler;
using Scheduler.NET.Core.Utils;
using System;

namespace DotnetSpider.Enterprise.Core.Scheduler
{
	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager : IJobManager
	{
		private readonly static ILogger Logger = LogCenter.GetLogger();

		/// <summary>
		/// 添加或者修改
		/// </summary>
		/// <param name="job"></param>
		public string AddOrUpdate(Job job)
		{
			try
			{
				Logger.Info($"Try to add or update job: {JsonConvert.SerializeObject(job)}.");
				job.Name = string.IsNullOrEmpty(job.Name) ? Guid.NewGuid().ToString("N") : job.Name;
				RecurringJob.AddOrUpdate<JobExecutor>(job.Name, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
				return job.Name;
			}
			catch (Exception e)
			{
				Logger.Error($"AddOrUpdate job failed: {e}");
				return string.Empty;
			}
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Remove(string jobId)
		{
			try
			{
				Logger.Info($"Try to remove job: {jobId}.");
				RecurringJob.RemoveIfExists(jobId);
			}
			catch (Exception e)
			{
				Logger.Error($"Remove job failed: {e}");
			}
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="jobId"></param>
		public void Trigger(string jobId)
		{
			try
			{
				Logger.Info($"Try to trigger job: {jobId}.");
				RecurringJob.Trigger(jobId);
			}
			catch (Exception e)
			{
				Logger.Error($"Trigger job failed: {e}");
			}
		}
	}
}
