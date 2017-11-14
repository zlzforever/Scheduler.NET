using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Utils
{
	public static class HttpUtil
	{
		private static readonly HttpClient httpClient = new HttpClient();

		public static HttpStatusCode Post(string url, string data)
		{
			var postData = $"data={WebUtility.HtmlEncode(data)}";
			var content = new StringContent(postData, Encoding.UTF8, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
			return httpClient.PostAsync(url, content).ContinueWith((task) =>
			{
				HttpResponseMessage response = task.Result;
				response.EnsureSuccessStatusCode();
				return response.StatusCode;
			}).Result;
		}
	}
}
