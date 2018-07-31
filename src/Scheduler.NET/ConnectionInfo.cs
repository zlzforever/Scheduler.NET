namespace Scheduler.NET
{
	public class ConnectionInfo
	{
		public string Id { get; set; }
		public string RemoteIp { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			var o = (ConnectionInfo)obj;
			if (Id != o.Id) return false;
			if (RemoteIp != o.RemoteIp) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return $"{Id}{RemoteIp}".GetHashCode();
		}
	}
}
