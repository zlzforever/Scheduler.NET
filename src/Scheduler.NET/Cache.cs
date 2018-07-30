using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Scheduler.NET
{
	public static class Cache
	{
		public static object CacheLocker = new object();
		public static readonly Dictionary<string, HashSet<string>> GroupMapConnections = new Dictionary<string, HashSet<string>>();
		public static readonly Dictionary<string, HashSet<string>> ConnectionIdMapClassNames = new Dictionary<string, HashSet<string>>();
	}
}
