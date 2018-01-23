using Hangfire;
using Jil;
using Microsoft.Extensions.Logging;
using Scheduler.NET.Core;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;
using System;

namespace DotnetSpider.Enterprise.Core.JobManager
{
	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager<T, E> : IJobManager<T> where T : IJob where E : IJobExecutor<T>
	{
		private readonly ILogger _logger;

		protected HangFireJobManager(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HangFireCallbackJobManager>();
		}

		public string Add(T job)
		{
			try
			{
				_logger.LogInformation($"Add job: {job}.");
				job.Name = string.IsNullOrEmpty(job.Name) ? Guid.NewGuid().ToString("N") : job.Name;
				RecurringJob.AddOrUpdate<E>(job.Name, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
				return job.Name;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Add job failed {JSON.Serialize(job)}.", e);
			}
		}

		public void Update(string id, T job)
		{
			if (string.IsNullOrWhiteSpace(id) || job == null)
			{
				throw new SchedulerException($"{nameof(id)} or {nameof(job)} should not be null.");
			}
			try
			{
				if (JobStorage.Current.GetMonitoringApi().JobDetails(id) == null)
				{
					throw new SchedulerException($"Job {nameof(id)} unfound.");
				}
				_logger.LogInformation($"Update job: {job}.");
				RecurringJob.AddOrUpdate<E>(id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Update job failed id: {id}, info: {JSON.Serialize(job)}.", e);
			}
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Remove(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new SchedulerException($"{nameof(id)} should not be null.");
			}
			try
			{
				_logger.LogInformation($"Remove job: {id}.");
				RecurringJob.RemoveIfExists(id);
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Remove job failed {id}.", e);
			}
		}

		/// <summary>
		/// 触发任务
		/// </summary>
		/// <param name="id"></param>
		public void Trigger(string id)
		{
			try
			{
				_logger.LogInformation($"Trigger job: {id}.");
				RecurringJob.Trigger(id);
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Trigger job failed {id}.", e);
			}
		}
	}

	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireCallbackJobManager : HangFireJobManager<CallbackJob, CallbackJobExecutor>
	{
		public HangFireCallbackJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
