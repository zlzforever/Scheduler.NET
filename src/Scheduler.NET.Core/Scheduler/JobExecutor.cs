using DotnetSpider.Enterprise.Core.Utils;
using Newtonsoft.Json;
using NLog;
using Scheduler.NET.Core.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

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
		private int _retryTimes = 5;

		public void Execute(string json)
		{
			try
			{
				Job job = JsonConvert.DeserializeObject<Job>(json);
				while (HttpUtil.Post(job.Url, job.Data) != HttpStatusCode.OK && _retryTimes > 0)
				{
					Thread.Sleep(3000);
					_retryTimes--;
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}
	}
}
