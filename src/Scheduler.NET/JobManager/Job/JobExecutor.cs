using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Scheduler.NET.Common;
using System;
using Dapper;
using Newtonsoft.Json;
using System.Linq;

namespace Scheduler.NET.JobManager.Job
{
	public class JobExecutor : JobExecutor<Common.Job>
	{
		private readonly IHubContext<ClientHub> _hubContext;
		private readonly ISchedulerOptions _options;
		private readonly ISchedulerNetCache _cache;

		public JobExecutor(ILoggerFactory loggerFactory, ISchedulerOptions options, IHubContext<ClientHub> hubContext, ISchedulerNetCache cache) : base(loggerFactory)
		{
			_hubContext = hubContext;
			_options = options;
			_cache = cache;
		}

		public override void Execute(Common.Job job)
		{
			var batchId = Guid.NewGuid().ToString("N");
			FireClients(job, batchId);
		}

		public void FireClients(Common.Job job, string batchId)
		{
			var jobContext = job.ToContext();

			var connections = _cache.GetConnectionFromGroup(jobContext.Group).AsList();
			if (connections != null && connections.Count > 0)
			{
				bool fired = false;
				foreach (var connection in connections)
				{
					var classNames = _cache.GetClassNames(connection.Id);

					if (classNames.Contains(jobContext.Name))
					{
						if (!fired)
						{
							fired = true;
						}
						Logger.LogInformation($"[{connection.RemoteIp}, {connection.Id}] trigger job '{job.Id}', batch '{batchId}', group '{job.Group}'.");
						_options.InsertJobHistory(batchId, job.Id, connection.RemoteIp, connection.Id);
						_hubContext.Clients.Client(connection.Id).SendAsync("Fire", jobContext, batchId);
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
