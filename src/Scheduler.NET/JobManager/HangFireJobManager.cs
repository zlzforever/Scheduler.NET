using System;
using Hangfire;
using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;
using Dapper;

namespace Scheduler.NET.JobManager
{
	public class HangFireJobManager : HangFireJobManager<Common.Job, JobExecutor>
	{
		public HangFireJobManager(ILoggerFactory loggerFactory, ISchedulerOptions options) : base(loggerFactory, options)
		{
		}
	}

	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager<TJob, TJobExecutor> : IJobManager<TJob> where TJob : IJob where TJobExecutor : IJobExecutor<TJob>
	{
		protected readonly ILogger _logger;
		protected readonly ISchedulerOptions _options;

		protected HangFireJobManager(ILoggerFactory loggerFactory, ISchedulerOptions options)
		{
			_logger = loggerFactory.CreateLogger<HangFireCallbackJobManager>();
			_options = options;
		}

		/// <summary>
		/// 创建任务
		/// </summary>
		/// <param name="job">任务</param>
		public string Create(TJob job)
		{
			if (job == null)
			{
				throw new ArgumentNullException($"{nameof(job)}");
			}
			job.Id = string.IsNullOrWhiteSpace(job.Id) ? Guid.NewGuid().ToString("N") : job.Id;
			_logger.LogInformation($"Create {job}.");
			InsertSchedulerJob(job);
			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);

			return job.Id;
		}

		protected virtual void InsertSchedulerJob(TJob job)
		{
			using (var conn = _options.CreateConnection())
			{
				var currentDatetimeSql = _options.GetCurrentDatetimeSql();
				conn?.Execute(
					$"INSERT INTO scheduler_job(id,{_options.GetGroupSql()},name,cron,content,jobtype,creationtime,lastmodificationtime) values (@Id,@Group,@Name,@Cron,@Content,'job',{currentDatetimeSql},{currentDatetimeSql})",
					job);
			}
		}

		protected virtual void UpdateSchedulerJob(TJob job)
		{
			using (var conn = _options.CreateConnection())
			{
				conn?.Execute(
					$"UPDATE job SET {_options.GetGroupSql()}=@Group,name=@Name,cron=@Cron,content=@Content,lastmodificationtime={_options.GetCurrentDatetimeSql()} WHERE id=@Id",
					job);
			}
		}

		/// <summary>
		/// 更新任务
		/// </summary>
		/// <param name="job">任务</param>
		public void Update(TJob job)
		{
			if (job == null)
			{
				throw new ArgumentNullException($"{nameof(job)}");
			}
			if (string.IsNullOrWhiteSpace(job.Id))
			{
				throw new ArgumentNullException($"{nameof(job.Id)}");
			}
			_logger.LogInformation($"Update {job}.");
			UpdateSchedulerJob(job);
			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Delete(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentNullException($"{nameof(id)}");
			}
			_logger.LogInformation($"Delete {id}.");
			using (var conn = _options.CreateConnection())
			{
				conn?.Execute(
					"DELETE FROM job WHERE id=@Id", new { Id = id });
			}
			RecurringJob.RemoveIfExists(id);
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="id"></param>
		public void Fire(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentNullException($"{nameof(id)}");
			}
			_logger.LogInformation($"Trigger {id}.");
			RecurringJob.Trigger(id);
		}

	}
}
