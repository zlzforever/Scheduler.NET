using System;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;
using Hangfire.SqlServer;

namespace Scheduler.NET.JobManager
{
	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager<TJob, TJobExecutor> : IJobManager<TJob> where TJob : BaseJob where TJobExecutor : IJobExecutor<TJob>
	{
		private readonly ILogger _logger;

		protected HangFireJobManager(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HangFireCallbackJobManager>();
		}

		public string Create(TJob job)
		{
			if (job == null)
			{
				_logger.LogError("Can't create null job.");
				return string.Empty;
			}
			_logger.LogInformation($"Create job {job}.");
			job.Id = string.IsNullOrWhiteSpace(job.Id) ? Guid.NewGuid().ToString("N") : job.Id;
			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
			return job.Id;
		}

		public void Update(TJob job)
		{
			if (job == null || string.IsNullOrWhiteSpace(job.Id))
			{
				throw new SchedulerException($"Can't update job {JsonConvert.SerializeObject(job)}.");
			}
			_logger.LogInformation($"Update job {job}.");
			//using (var conn = JobStorage.Current.GetConnection())
			//{
			//	// 这里是否需要考虑性能
			//	var entries = conn.GetAllEntriesFromHash($"recurring-job:{job.Id}");
			//	if (entries == null || !conn.GetAllEntriesFromHash($"recurring-job:{job.Id}").Any())
			//	{
			//		throw new SchedulerException($"Job {nameof(job.Id)} unfound.");
			//	}
			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
			//}
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Delete(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				_logger.LogInformation("Cant' delete null job.");
				return;
			}
			_logger.LogInformation($"Remove job {id}.");
			RecurringJob.RemoveIfExists(id);
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="id"></param>
		public void Trigger(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				_logger.LogInformation("Can't trigger null job.");
				return;
			}
			_logger.LogInformation($"Trigger job: {id}.");
			RecurringJob.Trigger(id);
		}
	}
}
