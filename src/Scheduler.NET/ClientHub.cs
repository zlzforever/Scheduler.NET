using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scheduler.NET.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Scheduler.NET
{
	public class ClientHub : Hub
	{
		private const string _groupHeaderName = "Group";
		private readonly ISchedulerOptions _options;
		private readonly ILogger _logger;
		private readonly ISchedulerNetCache _cache;
		private HttpRequest _request => Context.GetHttpContext().Request;

		public ClientHub(ISchedulerOptions options, ILoggerFactory loggerFactory, ISchedulerNetCache cache)
		{
			_options = options;
			_logger = loggerFactory.CreateLogger<ClientHub>();
			_cache = cache;
		}

		public override Task OnConnectedAsync()
		{
			var remoteIp = Context.GetRemoteIpAddress();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] connected.");
			if (!(IsAuth() && _request.Query.ContainsKey(_groupHeaderName)))
			{
				_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] auth denied.");
				Context.Abort();
				return Task.CompletedTask;
			}

			return base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var remoteIp = Context.GetRemoteIpAddress();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] disconnected.");
			var connectionId = Context.ConnectionId;

			_cache.RemoveClassNames(connectionId);

			var group = _request.Query[_groupHeaderName];

			//await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

			_cache.RemoveConnectionFromGroup(group, new ConnectionInfo { RemoteIp = remoteIp, Id = connectionId });
			await base.OnDisconnectedAsync(exception);
		}

		public async Task Watch(string[] classNames)
		{
			var remoteIp = Context.GetRemoteIpAddress();
			try
			{
				if (classNames == null || classNames.Length == 0)
				{
					_logger.LogWarning($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)}.");
					await Clients.Caller.SendAsync("WatchCallback", false);
					return;
				}

				_cache.SetClassNames(Context.ConnectionId, classNames);

				var group = _request.Query[_groupHeaderName];
				_cache.AddConnectionToGroup(group, new ConnectionInfo { RemoteIp = remoteIp, Id = Context.ConnectionId });

				// await Groups.AddToGroupAsync(Context.ConnectionId, group);

				_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)} success.");
				await Clients.Caller.SendAsync("WatchCallback", true);
			}
			catch (Exception e)
			{
				_logger.LogError($"[{remoteIp}, {Context.ConnectionId}] watched {JsonConvert.SerializeObject(classNames)} failed: {e}.");
				await Clients.Caller.SendAsync("WatchCallback", false);
			}
		}

		public Task FireCallback(string batchId, string jobId, JobStatus status)
		{
			var remoteIp = Context.GetRemoteIpAddress();
			_logger.LogInformation($"[{remoteIp}, {Context.ConnectionId}] notice job '{jobId}', batch '{batchId}', status '{status.ToString().ToLower()}'.");
			_options.ChangeJobHistoryStatus(batchId, jobId, remoteIp, Context.ConnectionId, status);
			return Task.CompletedTask;
		}

		private bool IsAuth()
		{
			if (!_options.UseToken)
			{
				return true;
			}
			if (_request.Headers.ContainsKey(_options.TokenHeader))
			{
				var token = _request.Headers[_options.TokenHeader].ToString();
				return _options.Tokens.Contains(token);
			}
			else
			{
				return false;
			}
		}
	}
}
