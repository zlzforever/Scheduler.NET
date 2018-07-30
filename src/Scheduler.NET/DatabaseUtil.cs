using Dapper;
using MySql.Data.MySqlClient;
using Scheduler.NET.Common;
using System.Data;
using System.Data.SqlClient;

namespace Scheduler.NET
{
	public static class DatabaseUtil
	{
		public static IDbConnection CreateConnection(this ISchedulerOptions options)
		{
			switch (options.HangfireStorageType.ToLower())
			{
				case "sqlserver":
					{
						return new SqlConnection(options.HangfireConnectionString);
					}
				case "mysql":
					{
						return new MySqlConnection(options.HangfireConnectionString);
					}
				default:
					{
						return null;
					}
			}
		}

		public static void ChangeJobHistoryStatus(this ISchedulerOptions options, string batchId, string jobId, string clientIp, string connectionId, JobStatus status)
		{
			using (var conn = options.CreateConnection())
			{
				conn.Execute($"UPDATE scheduler_job_history SET status = @Status, lastmodificationtime={options.GetCurrentDatetimeSql()} WHERE batchid=@BatchId AND jobid=@JobId AND clientip=@ClientIp AND connectionid=@ConnectionId", new
				{
					BatchId = batchId,
					JobId = jobId,
					Status = status,
					ClientIp = clientIp,
					ConnectionId = connectionId
				});
			}
		}

		public static void InsertJobHistory(this ISchedulerOptions options, string batchId, string jobId, string clientIp = null, string connectionId = null)
		{
			using (var conn = options.CreateConnection())
			{
				var currentDatetimeSql = options.GetCurrentDatetimeSql();
				conn.Execute($"INSERT INTO scheduler_job_history (batchid, jobid, status,clientip,connectionid,creationtime,lastmodificationtime) values (@BatchId,@JobId,@Status,@ClientIp,@ConnectionId,{currentDatetimeSql},{currentDatetimeSql})",
					new
					{
						BatchId = batchId,
						JobId = jobId,
						Status = JobStatus.Fire,
						ClientIp = clientIp,
						ConnectionId = connectionId
					});
			}
		}

		public static string GetCurrentDatetimeSql(this ISchedulerOptions options)
		{
			switch (options.HangfireStorageType.ToLower())
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

		public static string GetGroupSql(this ISchedulerOptions options)
		{
			switch (options.HangfireStorageType.ToLower())
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
	}
}
