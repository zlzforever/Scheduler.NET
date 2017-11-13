using DotnetSpider.Enterprise.Core.Propertities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Utils
{

	public class HttpUtil
	{
		private static HttpUtil _instance = null;
		private static object _lock = new object();
		private static readonly HttpClient httpClient = new HttpClient();

		public HttpUtil()
		{
			lock (_lock)
			{
				_instance = _instance ?? new HttpUtil();
			}
		}

		public static HttpStatusCode PostUrl(string url, string json)
		{
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var code = default(HttpStatusCode);
			httpClient.PostAsync(url, content).ContinueWith((task) =>
			{
				try
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();
					code = response.StatusCode;
				}
				catch (Exception ex)
				{

				}
			});
			return code;
		}

		public static async void GetUrl(string url)
		{
			await httpClient.GetAsync(url).ContinueWith((task) =>
			{
				try
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();

					var result = response.Content.ReadAsStringAsync().Result;
					if (!string.IsNullOrEmpty(result))
					{
					}
				}
				catch (Exception ex)
				{
				}
			});
		}


	}
}
