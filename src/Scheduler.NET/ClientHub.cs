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

namespace Scheduler.NET
{
	public class ClientHub : Hub
	{
		private readonly ISchedulerOptions _options;
		private readonly string GroupName = "Group";

		public HttpRequest Request => Context.GetHttpContext().Request;

		public ClientHub(ISchedulerOptions options)
		{
			_options = options;
		}

		public override Task OnConnectedAsync()
		{
			var isValid = IsAuth() && Request.Query.ContainsKey(GroupName);
			if (!isValid)
			{
				Context.Abort();
				return null;
			}
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			if (ClientCache.ConnectionMapClassNames.ContainsKey(Context.ConnectionId))
			{
				ClientCache.ConnectionMapClassNames.TryRemove(Context.ConnectionId, out _);
			}
			return base.OnDisconnectedAsync(exception);
		}

		public async Task Watch(string[] classNames)
		{
			bool cacheClassNames = false;
			if (!ClientCache.ConnectionMapClassNames.ContainsKey(Context.ConnectionId))
			{
				cacheClassNames = ClientCache.ConnectionMapClassNames.TryAdd(Context.ConnectionId, new HashSet<string>(classNames));
			}
			else
			{
				if (ClientCache.ConnectionMapClassNames.TryGetValue(Context.ConnectionId, out var oldValue))
				{
					cacheClassNames = ClientCache.ConnectionMapClassNames.TryUpdate(Context.ConnectionId, new HashSet<string>(classNames), oldValue);
				}
			}
			bool cacheGroup = false;
			var group = Request.Query[GroupName];
			HashSet<string> connections;
			if (!ClientCache.GroupMapConnections.ContainsKey(group))
			{
				connections = new HashSet<string>();
				ClientCache.GroupMapConnections.TryAdd(group, connections);
			}
			else
			{
				ClientCache.GroupMapConnections.TryGetValue(group, out connections);
			}
			if (connections != null)
			{
				connections.Add(Context.ConnectionId);
				cacheGroup = true;
			}
			await Clients.Caller.SendAsync("WatchCallback", cacheClassNames && cacheGroup);
		}

		public async Task FireCallback(string batchId, string jobId, JobStatus status)
		{
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
