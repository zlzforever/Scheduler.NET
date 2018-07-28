using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	public class SchedulerNETHelper
	{
		private readonly string _host;
		private readonly string _version;
		private static readonly HttpClient httpClient = new HttpClient();
		private readonly string _token;

		public string TokenHeader { get; set; } = "SchedulerNET";

		public SchedulerNETHelper(string host, string token = null, string version = "v1.0")
		{
			_host = new Uri(host).ToString();
			_version = version;
			_token = token;
		}

		public string Create<T>(T job) where T : IJob
		{
			try
			{
				var jobType = typeof(T).Name.ToLower();
				var url = $"{_host}api/{_version}/{jobType}";
				var msg = new HttpRequestMessage(HttpMethod.Post, url);
				AddTokenHeader(msg);
				job.Id = null;
				msg.Content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json");
				var response = httpClient.SendAsync(msg).Result;
				return CheckResult(response);
			}
			catch (SchedulerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Create scheduler failed: {e}");
			}
		}

		public void Delete<T>(string id) where T : IJob
		{
			try
			{
				var jobType = typeof(T).Name.ToLower();
				var url = $"{_host}api/{_version}/{jobType}/{id}";
				var msg = new HttpRequestMessage(HttpMethod.Delete, url);
				AddTokenHeader(msg);
				var response = httpClient.SendAsync(msg).Result;
				CheckResult(response);
			}
			catch (SchedulerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Delete scheduler failed: {e}");
			}
		}

		public void Trigger<T>(string id) where T : IJob
		{
			try
			{
				var jobType = typeof(T).Name.ToLower();
				var url = $"{_host}api/{_version}/{jobType}/{id}";
				var msg = new HttpRequestMessage(HttpMethod.Get, url);
				AddTokenHeader(msg);
				var response = httpClient.SendAsync(msg).Result;
				CheckResult(response);
			}
			catch (SchedulerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Trigger scheduler failed: {e}");
			}
		}

		public void Update<T>(T job) where T : IJob
		{
			try
			{
				var jobType = typeof(T).Name.ToLower();
				var url = $"{_host}api/{_version}/{jobType}";
				var msg = new HttpRequestMessage(HttpMethod.Put, url);
				msg.Content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json");
				AddTokenHeader(msg);
				var response = httpClient.SendAsync(msg).Result;
				CheckResult(response);
			}
			catch (SchedulerException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new SchedulerException($"Update scheduler failed: {e}");
			}
		}

		private string CheckResult(HttpResponseMessage response)
		{
			response.EnsureSuccessStatusCode();
			var str = response.Content.ReadAsStringAsync().Result;
			var json = JsonConvert.DeserializeObject<StandardResult>(str);
			if (json.Status != Status.Success)
			{
				throw new SchedulerException(json.Message);
			}
			return json.Data?.ToString();
		}

		private void AddTokenHeader(HttpRequestMessage msg)
		{
			if (!string.IsNullOrWhiteSpace(_token))
			{
				msg.Headers.Add(TokenHeader, _token);
			}
		}
	}
}
