using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Scheduler.NET.Common
{
	public struct ObjectKey
	{
		public object Key { get; private set; }

		public ObjectKey(object key)
		{
			Key = key;
		}

		public override int GetHashCode()
		{
			return Key == null ? -1 : Key.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			var o = (ObjectKey)obj;
			if (!Equals(Key?.ToString().ToMd5(), o.Key?.ToString().ToMd5())) return false;

			return true;
		}
	}

	public class HashObjectPool<T> where T : class
	{
		private class ObjectWrapper : IDisposable
		{
			public T Element { get; }
			public DateTime LastUseTime { get; private set; }

			public ObjectWrapper(T element)
			{
				Element = element;
				LastUseTime = DateTime.Now;
			}

			public void ResetUseTime()
			{
				LastUseTime = DateTime.Now;
			}

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}
		}

		private readonly Dictionary<ObjectKey, ObjectWrapper> _caches = new Dictionary<ObjectKey, ObjectWrapper>();
		private int _getCounter;

		public Func<ObjectKey, T> CreateFunc;

		public int CleanInterval { get; set; } = 1000;

		/// <summary>
		/// Timeout: second
		/// </summary>
		public int Timeout { get; set; } = 60;

		public HashObjectPool()
		{
		}

		public HashObjectPool(Func<ObjectKey, T> createFunc)
		{
			CreateFunc = createFunc;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public T Get(ObjectKey hash)
		{
			++_getCounter;

			if (_getCounter % CleanInterval == 0)
			{
				CleanTimeoutObject();
			}
			if (_caches.ContainsKey(hash))
			{
				return _caches[hash].Element;
			}
			else
			{
				var t = CreateFunc(hash);
				_caches.Add(hash, new ObjectWrapper(t));
				return t;
			}
		}

		private void CleanTimeoutObject()
		{
			var list = new List<ObjectKey>();
			var now = DateTime.Now;
			foreach (var kv in _caches)
			{
				if ((DateTime.Now - kv.Value.LastUseTime).TotalSeconds > Timeout)
				{
					list.Add(kv.Key);
				}
			}

			foreach (var k in list)
			{
				var v = _caches[k];
				v.Dispose();
				_caches.Remove(k);
			}
		}
	}
}
