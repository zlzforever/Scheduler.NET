using Microsoft.AspNetCore.SignalR;

namespace Scheduler.NET
{
	public static class HubCallerContextExtensions
	{
		public static string GetRemoteIpAddress(this HubCallerContext context)
		{
			return context.GetHttpContext().Connection.RemoteIpAddress.MapToIPv4().ToString();
		}
	}
}
