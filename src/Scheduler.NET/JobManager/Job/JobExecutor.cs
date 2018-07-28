using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Scheduler.NET.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;

namespace Scheduler.NET.JobManager.Job
{
	public class JobExecutor : JobExecutor<Common.Job>
	{
		private readonly IHubContext<ClientHub> _hubContext;
		private readonly ISchedulerOptions _options;

		public JobExecutor(ILoggerFactory loggerFactory, ISchedulerOptions options, IHubContext<ClientHub> hubContext) : base(loggerFactory)
		{
			_hubContext = hubContext;
			_options = options;
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

		public override void Execute(Common.Job job)
		{
			Logger.LogInformation($"Execute job {JsonConvert.SerializeObject(job)}.");
			var batchId = Guid.NewGuid().ToString("N");
			using (var conn = CreateConnection())
			{
				conn.Execute($"INSERT INTO scheduler_job_history (batchid, jobid, status,creationtime,lastmodificationtime) values (@BatchId,@JobId,@Status,{GetTimeSql()},{GetTimeSql()})",
					new
					{
						BatchId = batchId,
						JobId = job.Id,
						Status = 0
					});
			}
			_hubContext.Fire(job, batchId);
		}
	}

	public abstract class JobExecutor<T> : IJobExecutor<T> where T : IJob
	{
		protected ILogger Logger { get; }

		protected JobExecutor(ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger(GetType());
		}

		public abstract void Execute(T job);
	}
}
