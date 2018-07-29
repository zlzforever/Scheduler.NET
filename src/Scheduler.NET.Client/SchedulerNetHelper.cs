using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	public class SchedulerNetHelper
	{
		private static readonly HttpClient HttpClient = new HttpClient();
		private readonly string _host;
		private readonly string _version;
		private readonly string _token;

		public string TokenHeader { get; set; } = "SchedulerNET";

		public SchedulerNetHelper(string host, string token = null, string version = "v1.0")
		{
			_host = new Uri(host).ToString();
			_version = version;
			_token = token;
		}

		public string CreateJob(Job job)
		{
			return Create("job", job);
		}

		public string CreateCallbackJob(CallbackJob job)
		{
			return Create("callbackjob", job);
		}

		public void UpdateJob(Job job)
		{
			Update("job", job);
		}

		public void UpdateCallbackJob(CallbackJob job)
		{
			Update("callbackjob", job);
		}

		public void Delete(string jobType, string id)
		{
			var url = $"{_host}api/{_version}/{jobType}/{id}";
			var msg = new HttpRequestMessage(HttpMethod.Delete, url);
			AddTokenHeader(msg);
			var response = HttpClient.SendAsync(msg).Result;
			CheckResult(response);
		}

		public void FireJob(string id)
		{
			Fire("job", id);
		}

		public void FireCallbackJob(string id)
		{
			Fire("callbackjob", id);
		}

		private string Create(string jobType, IJob job)
		{
			var url = $"{_host}api/{_version}/{jobType}";
			var msg = new HttpRequestMessage(HttpMethod.Post, url);
			AddTokenHeader(msg);
			job.Id = null;
			msg.Content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json");
			var response = HttpClient.SendAsync(msg).Result;
			return CheckResult(response);
		}

		private void Fire(string jobType, string id)
		{
			var url = $"{_host}api/{_version}/{jobType}/{id}";
			var msg = new HttpRequestMessage(HttpMethod.Get, url);
			AddTokenHeader(msg);
			var response = HttpClient.SendAsync(msg).Result;
			CheckResult(response);
		}

		private void Update(string jobType, IJob job)
		{
			var url = $"{_host}api/{_version}/{jobType}";
			var msg = new HttpRequestMessage(HttpMethod.Put, url)
			{
				Content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json")
			};
			AddTokenHeader(msg);
			var response = HttpClient.SendAsync(msg).Result;
			CheckResult(response);
		}

		private string CheckResult(HttpResponseMessage response)
		{
			response.EnsureSuccessStatusCode();
			var str = response.Content.ReadAsStringAsync().Result;
			var json = JsonConvert.DeserializeObject<StandardResult>(str);
			if (!json.Success)
			{
				throw new SchedulerNetException(json.Message);
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
