using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Scheduler.NET.Common;
using System;
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

		public override void Execute(Common.Job job)
		{
			var batchId = Guid.NewGuid().ToString("N");
			Logger.LogInformation($"Execute job {JsonConvert.SerializeObject(job)}, batch {batchId}.");
			FireClients(job, batchId);
		}

		public void FireClients(Common.Job job, string batchId)
		{
			var jobContext = job.ToContext();
			if (Cache.GroupMapConnections.TryGetValue(jobContext.Group, out var connectionInfos))
			{
				bool fired = false;
				foreach (var connectionInfo in connectionInfos)
				{
					if (Cache.ConnectionIdMapClassNames.TryGetValue(connectionInfo.ConnectionId, out var classNames))
					{
						if (classNames.Contains(jobContext.Name))
						{
							if (!fired)
							{
								fired = true;
							}
							Logger.LogInformation($"Fire job {job.Id}, batch {batchId} on clientip {connectionInfo.ClientIp}, connectionid {connectionInfo.ConnectionId}.");
							_options.InsertJobHistory(batchId, job.Id, connectionInfo.ClientIp, connectionInfo.ConnectionId);
							_hubContext.Clients.Client(connectionInfo.ConnectionId).SendAsync("Fire", jobContext, batchId);
						}
					}
				}

				if (!fired)
				{
					Logger.LogInformation($"No client watch job {job.Name}.");
				}
			}
			else
			{
				throw new SchedulerNetException("No client connected.");
			}
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
