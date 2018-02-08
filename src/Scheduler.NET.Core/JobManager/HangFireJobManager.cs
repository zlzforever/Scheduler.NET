using Hangfire;
using Jil;
using Microsoft.Extensions.Logging;
using Scheduler.NET.Core;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;
using System;
using System.Linq;

namespace DotnetSpider.Enterprise.Core.JobManager
{
	/// <summary>
	/// 调度任务管理器
	/// </summary>
	public class HangFireJobManager<T, E> : IJobManager<T> where T : BaseJob where E : IJobExecutor<T>
	{
		private readonly ILogger _logger;

		protected HangFireJobManager(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HangFireCallbackJobManager>();
		}

		public string Create(T job)
		{
			try
			{
				_logger.LogInformation($"Add job {job}.");
				job.Id = string.IsNullOrEmpty(job.Id) ? Guid.NewGuid().ToString("N") : job.Id;
				RecurringJob.AddOrUpdate<E>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
				return job.Name;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Add job {JSON.Serialize(job)} failed.", e);
			}
		}

		public void Update(T job)
		{
			if (string.IsNullOrWhiteSpace(job.Id) || job == null)
			{
				throw new SchedulerException($"{nameof(job.Id)} or {nameof(job)} must not be null/empty.");
			}
			try
			{
				_logger.LogInformation($"Update job {job}.");
				using (var conn = JobStorage.Current.GetConnection())
				{
					// 这里是否需要考虑性能
					var entries = conn.GetAllEntriesFromHash($"recurring-job:{job.Id}");
					if (entries == null || conn.GetAllEntriesFromHash($"recurring-job:{job.Id}").Any())
					{
						throw new SchedulerException($"Job {nameof(job.Id)} unfound.");
					}
					RecurringJob.AddOrUpdate<E>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
				}
			}
			catch (SchedulerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Update job failed id: {job.Name}, info: {JSON.Serialize(job)}.", e);
			}
		}

		/// <summary>
		/// 删除计划任务
		/// </summary>
		public void Delete(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new SchedulerException($"{nameof(id)} must not be null/empty.");
			}
			try
			{
				_logger.LogInformation($"Remove job {id}.");
				RecurringJob.RemoveIfExists(id);
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Remove job {id} failed.", e);
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
				throw new SchedulerException($"Trigger job {id} failed.", e);
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

	public class HangFireRedisJobManager : HangFireJobManager<RedisJob, RedisJobExecutor>
	{
		public HangFireRedisJobManager(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}
	}
}
