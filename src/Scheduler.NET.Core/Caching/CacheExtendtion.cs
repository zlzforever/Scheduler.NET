using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Core.Caching
{

	/// <summary>
	/// 缓存扩展方法
	/// </summary>
    public static class CacheExtendtion
    {
		private static int DefaultCacheTimeMinutes { get { return 60; } }
		
		public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
		{
			//use default cache time
			return Get(cacheManager, key, DefaultCacheTimeMinutes, acquire);
		}
		
		public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
		{
			if (cacheManager.IsSet(key))
				return cacheManager.Get<T>(key);
			
			var result = acquire();
			if (cacheTime > 0)
				cacheManager.Set(key, result, cacheTime);

			return result;
		}
		
		public static void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
		{
			var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
			var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();
			
			matchesKeys.ForEach(cacheManager.Remove);
		}
	}
}
