namespace Scheduler.NET
{
	public class ConnectionInfo
	{
		public string ConnectionId { get; set; }
		public string ClientIp { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			var o = (ConnectionInfo)obj;
			if (ConnectionId != o.ConnectionId) return false;
			if (ClientIp != o.ClientIp) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return $"{ConnectionId}{ClientIp}".GetHashCode();
		}
	}
}
