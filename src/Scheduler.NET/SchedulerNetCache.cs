using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Scheduler.NET
{
	public interface ISchedulerNetCache
	{
		void SetConnectionClassNames(string connectionId, string[] classNames);
		string[] GetConnectionClassNames(string connectionId);
		void AddConnectionInfoToGroup(string group, ConnectionInfo connectionInfo);
		IEnumerable<ConnectionInfo> GetConnectionInfoFromGroup(string group);
		void RemoveConnectionClassNames(string connectionId);
		void RemoveConnectionInfoFromGroup(string group, ConnectionInfo connectionInfo);
	}

	public class MemorySchedulerNetCache : ISchedulerNetCache
	{
		private readonly Dictionary<string, HashSet<ConnectionInfo>> GroupMapConnections = new Dictionary<string, HashSet<ConnectionInfo>>();
		private readonly Dictionary<string, string[]> ConnectionIdMapClassNames = new Dictionary<string, string[]>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public string[] GetConnectionClassNames(string connectionId)
		{
			return ConnectionIdMapClassNames.ContainsKey(connectionId) ? ConnectionIdMapClassNames[connectionId] : new string[0];
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetConnectionClassNames(string connectionId, string[] classNames)
		{
			if (ConnectionIdMapClassNames.ContainsKey(connectionId))
			{
				ConnectionIdMapClassNames[connectionId] = classNames;
			}
			else
			{
				ConnectionIdMapClassNames.Add(connectionId, classNames);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddConnectionInfoToGroup(string group, ConnectionInfo connectionInfo)
		{
			if (!GroupMapConnections.ContainsKey(group))
			{
				GroupMapConnections.Add(group, new HashSet<ConnectionInfo>());
			}
			GroupMapConnections[group].Add(connectionInfo);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<ConnectionInfo> GetConnectionInfoFromGroup(string group)
		{
			return GroupMapConnections.ContainsKey(group) ? GroupMapConnections[group] : Enumerable.Empty<ConnectionInfo>();
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveConnectionInfoFromGroup(string group, ConnectionInfo connectionInfo)
		{
			if (GroupMapConnections.ContainsKey(group))
			{
				GroupMapConnections[group].Remove(connectionInfo);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveConnectionClassNames(string connectionId)
		{
			ConnectionIdMapClassNames.Remove(connectionId);
		}
	}

	public class RedisSchedulerNetCache : ISchedulerNetCache
	{
		private readonly ISchedulerOptions _options;
		private readonly ConnectionMultiplexer _multiplexer;
		private readonly IDatabase _database;

		public RedisSchedulerNetCache(ISchedulerOptions options)
		{
			_options = options;
			_multiplexer = ConnectionMultiplexer.Connect(_options.Cache.ConnectionString);
			_database = _multiplexer.GetDatabase(0);
		}

		public void AddConnectionInfoToGroup(string group, ConnectionInfo connectionInfo)
		{
			_database.SetAdd(GetGroupKey(group), JsonConvert.SerializeObject(connectionInfo));
		}

		public string[] GetConnectionClassNames(string connectionId)
		{
			var result = new List<string>();
			var caches = _database.SetMembers(GetClassNamesKey(connectionId));
			foreach (var cache in caches)
			{
				if (cache.HasValue)
				{
					result.Add(cache);
				}
			}
			return result.ToArray();
		}

		public IEnumerable<ConnectionInfo> GetConnectionInfoFromGroup(string group)
		{
			var result = new List<ConnectionInfo>();
			var caches = _database.SetMembers(GetGroupKey(group));
			foreach (var cache in caches)
			{
				if (cache.HasValue)
				{
					result.Add(JsonConvert.DeserializeObject<ConnectionInfo>(cache));
				}
			}
			return result;
		}

		public void RemoveConnectionClassNames(string connectionId)
		{
			_database.KeyDelete(GetClassNamesKey(connectionId));
		}

		public void RemoveConnectionInfoFromGroup(string group, ConnectionInfo connectionInfo)
		{
			_database.SetRemove(GetGroupKey(group), JsonConvert.SerializeObject(connectionInfo));
		}

		public void SetConnectionClassNames(string connectionId, string[] classNames)
		{
			_database.SetAdd(GetClassNamesKey(connectionId), classNames.Select(c => (RedisValue)c).ToArray());
		}

		private string GetClassNamesKey(string connectionId)
		{
			return $"scheduler.net:class_names_{connectionId}";
		}

		private string GetGroupKey(string group)
		{
			return $"scheduler.net:group_{group}";
		}
	}
}
