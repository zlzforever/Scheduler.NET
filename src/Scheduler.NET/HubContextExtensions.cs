using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Common;

namespace Scheduler.NET
{
	public static class HubContextExtensions
	{
		public static void Fire(this IHubContext<ClientHub> hubContext, ILogger logger, Job job, string batchId)
		{
			var jobContext = job.ToContext();
			if (Cache.GroupMapConnections.TryGetValue(jobContext.Group, out var connections))
			{
				bool fired = false;
				foreach (var connection in connections)
				{
					if (Cache.ConnectionIdMapClassNames.TryGetValue(connection, out var classNames))
					{
						if (classNames.Contains(jobContext.Name))
						{
							if (!fired)
							{
								fired = true;
							}
							
							hubContext.Clients.Client(connection).SendAsync("Fire", jobContext, batchId);
						}
					}
				}

				if (fired)
				{
					logger.LogInformation($"Execute {JsonConvert.SerializeObject(job)}, batch {batchId}.");
				}
				else
				{
					logger.LogInformation($"No client watch job {job.Name}, batch {batchId}.");
				}
			}
			else
			{
				throw new SchedulerNetException("No client connected.");
			}
		}
	}
}
