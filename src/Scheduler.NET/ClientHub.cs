using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scheduler.NET.Common;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Scheduler.NET
{
	public class ClientHub : Hub
	{
		private readonly ISchedulerOptions _options;
		private readonly string GroupName = "Group";
		private readonly ILogger _logger;

		public HttpRequest Request => Context.GetHttpContext().Request;

		public ClientHub(ISchedulerOptions options, ILoggerFactory loggerFactory)
		{
			_options = options;
			_logger = loggerFactory.CreateLogger<ClientHub>();
		}

		public override Task OnConnectedAsync()
		{
			var remoteIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] connected.");
			if (!(IsAuth() && Request.Query.ContainsKey(GroupName)))
			{
				_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] auth denied.");
				Context.Abort();
				return Task.CompletedTask;
			}
			return base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var remoteIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] disconnected.");
			var connectionId = Context.ConnectionId;
			// TODO: 不使用 lock 以优化性能
			lock (Cache.CacheLocker)
			{
				if (Cache.ConnectionIdMapClassNames.ContainsKey(connectionId))
				{
					Cache.ConnectionIdMapClassNames.Remove(connectionId);
				}

				foreach (var kv in Cache.GroupMapConnections)
				{
					kv.Value.Remove(connectionId);
				}
			}
			await base.OnDisconnectedAsync(exception);
		}

		public async Task Watch(string[] classNames)
		{
			var remoteIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
			try
			{
				if (classNames == null || classNames.Length == 0)
				{
					_logger.LogWarning($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)}.");
					await Clients.Caller.SendAsync("WatchCallback", false);
					return;
				}

				lock (Cache.CacheLocker)
				{
					if (!Cache.ConnectionIdMapClassNames.ContainsKey(Context.ConnectionId))
					{
						Cache.ConnectionIdMapClassNames.Add(Context.ConnectionId, new HashSet<string>(classNames));
					}
					else
					{
						Cache.ConnectionIdMapClassNames[Context.ConnectionId] = new HashSet<string>(classNames);
					}

					var group = Request.Query[GroupName];

					if (!Cache.GroupMapConnections.ContainsKey(group))
					{
						Cache.GroupMapConnections.Add(group, new HashSet<string> { Context.ConnectionId });
					}
					else
					{
						Cache.GroupMapConnections[group].Add(Context.ConnectionId);
					}
				}
				_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)} success.");
				await Clients.Caller.SendAsync("WatchCallback", true);
			}
			catch (Exception e)
			{
				_logger.LogError($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)} failed: {e}.");
				await Clients.Caller.SendAsync("WatchCallback", false);
			}
		}

		public async Task FireCallback(string batchId, string jobId, JobStatus status)
		{
			var remoteIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] fire job {jobId}, batch {batchId} {status}.");
			using (var conn = CreateConnection())
			{
				await conn.ExecuteAsync($"UPDATE scheduler_job_history SET status = {(int)status}, lastmodificationtime={GetTimeSql()} WHERE batchid=@BatchId AND jobid=@JobId", new
				{
					BatchId = batchId,
					JobId = jobId
				});
			}
		}

		public async Task Complete(string batchId, string jobId, bool success)
		{
			var remoteIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] complete job {jobId}, batch {batchId} {(success ? "success" : "failed")}.");
			using (var conn = CreateConnection())
			{
				if (success)
				{
					await conn.ExecuteAsync($"UPDATE scheduler_job_history SET status = {(int)JobStatus.Success}, lastmodificationtime={GetTimeSql()} WHERE batchid=@BatchId AND jobid=@JobId", new
					{
						BatchId = batchId,
						JobId = jobId,
					});
				}
				else
				{
					await conn.ExecuteAsync($"UPDATE scheduler_job_history SET status = {(int)JobStatus.Failed}, lastmodificationtime={GetTimeSql()} WHERE batchid=@BatchId AND jobid=@JobId", new
					{
						BatchId = batchId,
						JobId = jobId,
					});
				}
			}
		}

		private string GetTimeSql()
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

		private IDbConnection CreateConnection()
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

		private bool IsAuth()
		{
			if (!_options.UseToken)
			{
				return true;
			}
			if (Request.Headers.ContainsKey(_options.TokenHeader))
			{
				var token = Request.Headers[_options.TokenHeader].ToString();
				return _options.Tokens.Contains(token);
			}
			else
			{
				return false;
			}
		}
	}
}
