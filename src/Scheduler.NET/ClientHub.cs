using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.NET
{
	public class ClientHub : Hub
	{
		public static readonly Dictionary<string, IClientProxy> Hubs = new Dictionary<string, IClientProxy>();

		public override Task OnConnectedAsync()
		{
			if (Context.GetHttpContext().Request.Headers.ContainsKey("TOPIC"))
			{

			}
			if (!Hubs.ContainsKey("TOPIC1"))
			{
				Hubs.Add("TOPIC1", Clients.Caller);
			}

			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			if (Hubs.ContainsKey("TOPIC1"))
			{
				Hubs.Remove("TOPIC1");
			}
			return base.OnDisconnectedAsync(exception);
		}

 
		public async Task SendMessage(string user, string message)
		{
			await Clients.Caller.SendAsync("ReceiveMessage", user, message);
		}
	}
}
