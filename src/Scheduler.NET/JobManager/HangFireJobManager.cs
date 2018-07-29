using System;
using Hangfire;
using Microsoft.Extensions.Logging;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
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
		private readonly ILogger _logger;
		private readonly ISchedulerOptions _options;

		protected HangFireJobManager(ILoggerFactory loggerFactory, ISchedulerOptions options)
		{
			_logger = loggerFactory.CreateLogger<HangFireCallbackJobManager>();
			_options = options;
		}

		protected virtual IDbConnection CreateConnection()
		{
			switch (_options.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						return new SqlConnection(_options.HangfireConnectionString);
					}
				case "mysql":
					{
						return new MySqlConnection(_options.HangfireConnectionString);
					}
				default:
					{
						return null;
					}
			}
		}

		protected string GetTimeSql()
		{
			switch (_options.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						return "GETDATE()";
					}
				case "mysql":
					{
						return "CURRENT_TIMESTAMP()";
					}
				default:
					{
						return null;
					}
			}
		}

		protected string GetGroupSql()
		{
			switch (_options.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						return "[group]";
					}
				case "mysql":
					{
						return "`group`";
					}
				default:
					{
						return null;
					}
			}
		}

		public string Create(TJob job)
		{
			if (job == null)
			{
				throw new ArgumentNullException($"{nameof(job)}");
			}
			_logger.LogInformation($"Create {job}.");
			job.Id = string.IsNullOrWhiteSpace(job.Id) ? Guid.NewGuid().ToString("N") : job.Id;
			using (var conn = CreateConnection())
			{
				conn?.Execute(
					$"INSERT INTO scheduler_job(id,{GetGroupSql()},classname,cron,content,creationtime,lastmodificationtime) values (@Id,@Group,@ClassName,@Cron,@Content,{GetTimeSql()},{GetTimeSql()})",
					job);
			}

			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);

			return job.Id;
		}

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
			using (var conn = CreateConnection())
			{
				conn?.Execute(
					$"UPDATE job SET {GetGroupSql()}=@Group, classname=@ClassName, cron=@Cron, content=@Content, lastmodificationtime={GetTimeSql()} WHERE id=@Id",
					job);
			}
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
			_logger.LogInformation($"Remove {id}.");
			using (var conn = CreateConnection())
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
