using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Caching
{

	public class RedisCacheManager : ICacheManager
	{
		public T Get<T>(string _ket)
		{
			throw new NotImplementedException();
		}

		public bool IsSet(string _key)
		{
			throw new NotImplementedException();
		}

		public void Remove(string _key)
		{
			throw new NotImplementedException();
		}

		public void Set(string _key, object _value, int cacheTime)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}
	}
}
