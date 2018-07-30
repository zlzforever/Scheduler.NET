using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Scheduler.NET.Common;

namespace Scheduler.NET.Client
{
	/// <summary>
	/// Scheduler.NET Api
	/// </summary>
	public class SchedulerNetHelper
	{
		private static readonly HttpClient HttpClient = new HttpClient();
		private readonly string _host;
		private readonly string _version;
		private readonly string _token;

		/// <summary>
		/// Scheduler.NET 服务鉴权的 Header 名称
		/// </summary>
		public string TokenHeader { get; set; } = "SchedulerNET";

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="service">Scheduler.NET 服务地址</param>
		/// <param name="token">访问的 Token</param>
		/// <param name="version">Api 版本</param>
		public SchedulerNetHelper(string service, string token = null, string version = "v1.0")
		{
			_host = new Uri(service).ToString();
			_version = version;
			_token = token;
		}

		/// <summary>
		/// 创建普通任务
		/// </summary>
		/// <param name="job">任务信息</param>
		/// <returns>任务编号</returns>
		public string CreateJob(Job job)
		{
			return Create("job", job);
		}

		/// <summary>
		/// 创建回调任务
		/// </summary>
		/// <param name="job">回调任务信息</param>
		/// <returns>任务编号</returns>
		public string CreateCallbackJob(CallbackJob job)
		{
			return Create("callbackjob", job);
		}

		/// <summary>
		/// 更新普通任务
		/// </summary>
		/// <param name="job">任务</param>
		public void UpdateJob(Job job)
		{
			Update("job", job);
		}

		/// <summary>
		/// 更新回调任务
		/// </summary>
		/// <param name="job">任务</param>
		public void UpdateCallbackJob(CallbackJob job)
		{
			Update("callbackjob", job);
		}

		/// <summary>
		/// 删除普通任务
		/// </summary>
		/// <param name="id">任务编号</param>
		public void DeleteJob(string id)
		{
			Delete("job", id);
		}

		/// <summary>
		/// 删除回调任务
		/// </summary>
		/// <param name="id">任务编号</param>
		public void DeleteCallbackJob(string id)
		{
			Delete("callbackjob", id);
		}

		/// <summary>
		/// 触发普通任务
		/// </summary>
		/// <param name="id">任务编号</param>
		public void FireJob(string id)
		{
			Fire("job", id);
		}

		/// <summary>
		/// 触发回调任务
		/// </summary>
		/// <param name="id">任务编号</param>
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

		private void Delete(string jobType, string id)
		{
			var url = $"{_host}api/{_version}/{jobType}/{id}";
			var msg = new HttpRequestMessage(HttpMethod.Delete, url);
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

		private void Fire(string jobType, string id)
		{
			var url = $"{_host}api/{_version}/{jobType}/{id}";
			var msg = new HttpRequestMessage(HttpMethod.Get, url);
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
