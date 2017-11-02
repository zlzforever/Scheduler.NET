using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Caching
{

	/// <summary>
	/// 缓存接口
	/// </summary>
	public interface ICacheManager
	{
		T Get<T>(string _ket);

		void Set(string _key, object _value, int cacheTime);

		bool IsSet(string _key);

		void Remove(string _key);

		void Clear();
	}
}
