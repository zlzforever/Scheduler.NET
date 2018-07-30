using System.Collections.Generic;

namespace Scheduler.NET
{
	public static class Cache
	{
		public static object CacheLocker = new object();
		public static readonly Dictionary<string, HashSet<ConnectionInfo>> GroupMapConnections = new Dictionary<string, HashSet<ConnectionInfo>>();
		public static readonly Dictionary<string, HashSet<string>> ConnectionIdMapClassNames = new Dictionary<string, HashSet<string>>();
	}
}
