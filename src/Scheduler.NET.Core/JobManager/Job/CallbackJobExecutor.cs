using Jil;
using NLog;
using Polly;
using Polly.Retry;
using Scheduler.NET.Core.Utils;
using System;
using System.Net.Http;

namespace Scheduler.NET.Core.JobManager.Job
{
	/// <summary>
	/// 
	/// </summary>
	public class CallbackJobExecutor : IJobExecutor<CallbackJob>
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

		public void Execute(CallbackJob job)
		{
			try
			{
#if Release
				RetryPolicy.Execute(() =>
				{
					var response = HttpUtil.Post(job.Url, job.Data);
					response.EnsureSuccessStatusCode();
				});
#else
				Logger.Info($"Run callback job {JSON.Serialize(job)}");
#endif
			}
			catch (Exception e)
			{
				Logger.Error($"Execute {JSON.Serialize(job)} failed: {e}");
			}
		}
	}
}
