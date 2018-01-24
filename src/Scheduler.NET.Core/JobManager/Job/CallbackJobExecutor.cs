using Jil;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using DotnetSpider.Enterprise.Core.Utils;

namespace Scheduler.NET.Core.JobManager.Job
{
	/// <summary>
	/// 
	/// </summary>
	public class CallbackJobExecutor : BaseJobExecutor<CallbackJob>
	{
		private readonly RetryPolicy _retryPolicy;

		public CallbackJobExecutor() : base()
		{
			_retryPolicy = Policy.Handle<HttpRequestException>().Retry(RetryTimes, (ex, count) =>
			{
				_logger.LogError($"Execute callback job failed [{count}]: {ex}");
			});
		}

		public override void Execute(CallbackJob job)
		{
			try
			{
				_retryPolicy.Execute(() =>
				{
					var response = HttpUtil.Post(job.Url, job.Data);
					response.EnsureSuccessStatusCode();
				});

				_logger.LogInformation($"Execute callback job {JSON.Serialize(job)} success.");

			}
			catch (Exception e)
			{
				_logger.LogError($"Execute callback job {JSON.Serialize(job)} failed: {e}.");
			}
		}
	}
}
