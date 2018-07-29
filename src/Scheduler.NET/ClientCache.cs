using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Scheduler.NET
{
	public static class ClientCache
	{
		public static readonly ConcurrentDictionary<string, HashSet<string>> GroupMapConnections = new ConcurrentDictionary<string, HashSet<string>>();
		public static readonly ConcurrentDictionary<string, HashSet<string>> ConnectionMapClassNames = new ConcurrentDictionary<string, HashSet<string>>();
	}
}
