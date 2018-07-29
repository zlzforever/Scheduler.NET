using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using Scheduler.NET.Common;
using System.Net;
using System.Text;

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

		public CallbackJobExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override void Execute(CallbackJob job)
		{
			Logger.LogInformation($"Execute callback job {JsonConvert.SerializeObject(job)}.");

			var msg = new HttpRequestMessage(job.Method, job.Url)
			{
				Content = new StringContent(job.Content, Encoding.UTF8, "application/json")
			};
			var response = Client.SendAsync(msg).Result;
			response.EnsureSuccessStatusCode();
		}
	}
}
