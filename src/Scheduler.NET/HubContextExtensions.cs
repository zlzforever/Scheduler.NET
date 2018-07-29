using Microsoft.AspNetCore.SignalR;
using Scheduler.NET.Common;

namespace Scheduler.NET
{
	public static class HubContextExtensions
	{
		public static void Fire(this IHubContext<ClientHub> hubContext, Job job, string batchId)
		{
			var jobContext = job.ToContext();
			if (ClientCache.GroupMapConnections.TryGetValue(jobContext.Group, out var connections))
			{
				foreach (var connection in connections)
				{
					if (ClientCache.ConnectionMapClassNames.TryGetValue(connection, out var classNames))
					{
						if (classNames.Contains(jobContext.ClassName))
						{
							hubContext.Clients.Client(connection).SendAsync("Fire", jobContext, batchId);
						}
					}
				}
			}
			else
			{
				throw new SchedulerNetException("None client connected.");
			}
		}
	}
}
