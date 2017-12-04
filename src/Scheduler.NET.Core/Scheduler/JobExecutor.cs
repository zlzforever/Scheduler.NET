using DotnetSpider.Enterprise.Core.Utils;
using NLog;
using Polly;
using Polly.Retry;
using Scheduler.NET.Core.Utils;
using System;
using System.Net.Http;

namespace Scheduler.NET.Core.Scheduler
{
	/// <summary>
	/// 
	/// </summary>
	public class JobExecutor
	{
		private readonly static ILogger Logger = LogCenter.GetLogger();

		/// <summary>
		/// 重试次数
		/// </summary>
		private static int RetryTimes = 5;

		private readonly static RetryPolicy RetryPolicy = Policy.Handle<HttpRequestException>().Retry(RetryTimes, (ex, count) =>
		{
			Logger.Error($"Execute job failed [{count}]: {ex}");
		});

		public void Execute(Job job)
		{
			try
			{
				RetryPolicy.Execute(() =>
				{
					var response = HttpUtil.Post(job.Url, job.Data);
					response.EnsureSuccessStatusCode();
				});
			}
			catch (Exception e)
			{
				Logger.Error($"Execute job failed: {e}");
			}
		}
	}
}
