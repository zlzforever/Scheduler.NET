using Microsoft.AspNetCore.SignalR;
using Scheduler.NET.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET
{
	public static class HubContextExtensions
	{
		public static void Fire(this IHubContext<ClientHub> hubContext, Job job, string batchId)
		{
			var jobContext = job.ToJobContext();
			HashSet<string> connections;
			if (ClientCache.GroupMapConnections.TryGetValue(jobContext.Group, out connections))
			{
				foreach (var connection in connections)
				{
					HashSet<string> classNames;

					if (ClientCache.ConnectionMapClassNames.TryGetValue(connection, out classNames))
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
				// TODO: LOG
			}
		}
	}
}
