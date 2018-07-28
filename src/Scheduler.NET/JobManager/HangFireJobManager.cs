using System;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.JobManager.Job;
using Scheduler.NET.Common;
using Hangfire.SqlServer;
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
	public class HangFireJobManager<TJob, TJobExecutor> : IJobManager<TJob> where TJob : Common.Job where TJobExecutor : IJobExecutor<TJob>
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
				_logger.LogError("Can't create null job.");
				return string.Empty;
			}
			_logger.LogInformation($"Create job {job}.");
			job.Id = string.IsNullOrWhiteSpace(job.Id) ? Guid.NewGuid().ToString("N") : job.Id;
			using (var conn = CreateConnection())
			{
				if (conn != null)
				{
					conn.Execute(
$"INSERT INTO scheduler_job(id,{GetGroupSql()},classname,cron,content,creationtime,lastmodificationtime) values (@Id,@Group,@ClassName,@Cron,@Content,{GetTimeSql()},{GetTimeSql()})",
						job);
				}
			}

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
			RecurringJob.AddOrUpdate<TJobExecutor>(job.Id, x => x.Execute(job), job.Cron, TimeZoneInfo.Local);
			using (var conn = CreateConnection())
			{
				if (conn != null)
				{
					conn.Execute(
					$"UPDATE job SET {GetGroupSql()} = @Group, classname=@ClassName, cron=@Cron, content=@Content, lastmodificationtime={GetTimeSql()} where id=@Id",
					job);
				}
			}
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
			using (var conn = CreateConnection())
			{
				if (conn != null)
				{
					conn.Execute(
					"DELETE FROM job WHERE id=@Id", new { Id = id });
				}
			}
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
