using Jil;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using DotnetSpider.Enterprise.Core.Utils;

namespace Scheduler.NET.Core.JobManager.Job
{
	/// <summary>
	/// 
	/// </summary>
	public class CallbackJobExecutor : BaseJobExecutor<CallbackJob>
	{
		private readonly RetryPolicy _retryPolicy;

		public CallbackJobExecutor()
		{
			_retryPolicy = Policy.Handle<HttpRequestException>().Retry(RetryTimes, (ex, count) =>
			{
				Logger.LogError($"Execute callback job failed [{count}]: {ex}.");
			});
		}

		public override async void Execute(CallbackJob job)
		{
			try
			{
				await _retryPolicy.Execute(async () =>
				{
					var response = await HttpUtil.Post(job.Url, job.Data);
					response.EnsureSuccessStatusCode();
				});

				Logger.LogInformation($"Execute callback job {JSON.Serialize(job)} success.");

			}
			catch (Exception e)
			{
				Logger.LogError($"Execute callback job {JSON.Serialize(job)} failed: {e}.");
			}
		}
	}
}
