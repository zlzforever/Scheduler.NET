using DotnetSpider.Enterprise.Core.Propertities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Utils
{
	public class HttpUtil
	{
		private static HttpUtil _instance = null;
		private static object _lock = new object();

		public HttpUtil()
		{
			lock (_lock)
			{
				_instance = _instance ?? new HttpUtil();
			}
		}

		public static T RequestUrl<T>(string url, HttpMethod method) where T : class
		{
			var rent = default(T);
			var restp = new RestProperties
			{
				Url = url,
				Method = method,
			};

			var restc = new RestConnection(restp);
			var rentStr = restc.SendToApi();
			if (!string.IsNullOrEmpty(rentStr))
			{
				rent = JsonConvert.DeserializeObject<T>(rentStr);
			}
			return rent;
		}

	}
}
