using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Scheduler.NET.Common;
using System.Net;
using System.Text;
using System;
using Dapper;

namespace Scheduler.NET.JobManager.Job
{
	public class CallbackJobExecutor : JobExecutor<CallbackJob>
	{
		private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			UseCookies = false
		});

		private readonly ISchedulerOptions _options;

		public CallbackJobExecutor(ILoggerFactory loggerFactory, ISchedulerOptions options) : base(loggerFactory)
		{
			_options = options;
		}

		public override void Execute(CallbackJob job)
		{
			var batchId = Guid.NewGuid().ToString("N");
			Logger.LogInformation($"Execute callback job {JsonConvert.SerializeObject(job)}, batch {batchId}.");
			_options.InsertJobHistory(batchId, job.Id);
			var msg = new HttpRequestMessage(job.Method, job.Url)
			{
				Content = new StringContent(job.Content, Encoding.UTF8, "application/json")
			};
			var response = Client.SendAsync(msg).Result;
			response.EnsureSuccessStatusCode();
			_options.ChangeJobHistoryStatus(batchId, job.Id, null, null, JobStatus.Success);
		}
	}
}
