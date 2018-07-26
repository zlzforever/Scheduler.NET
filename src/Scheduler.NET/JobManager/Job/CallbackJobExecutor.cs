using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net.Http;
using DotnetSpider.Enterprise.Core.Utils;
using Newtonsoft.Json;
using Scheduler.NET.Common;

namespace Scheduler.NET.JobManager.Job
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class CallbackJobExecutor : BaseJobExecutor<CallbackJob>
	{
		public CallbackJobExecutor(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
		}

		public override void Execute(CallbackJob job)
		{
			try
			{
				Logger.LogInformation($"Execute callback job {JsonConvert.SerializeObject(job)}.");
				Policy.Handle<HttpRequestException>().Retry(RetryTimes, (ex, count) =>
				{
					Logger.LogError($"Execute callback job failed [{count}] {JsonConvert.SerializeObject(job)}: {ex}.");
				}).Execute(async () =>
				{
					var response = await HttpUtil.Get(job.Url);
					response.EnsureSuccessStatusCode();
				});
			}
			catch (Exception e)
			{
				Logger.LogError($"Execute callback job {JsonConvert.SerializeObject(job)} failed: {e}.");
			}
		}
	}
}
