using DotnetSpider.Enterprise.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
	public class RecurringJobService
	{

		private readonly ILogger<RecurringJobService> _Logger;

		/// <summary>
		/// 重试次数
		/// </summary>
		private  int _Times = 5;

		public RecurringJobService(ILogger<RecurringJobService> _logger)
		{
			this._Logger = _logger;
		}

		public void ExecuteJob(string json)
		{
			try
			{
				SpiderJob job = JsonConvert.DeserializeObject<SpiderJob>(json);
				var rent = HttpUtil.PostUrl(job.CallBack, json);
				while (rent != HttpStatusCode.OK && _Times > 0)
				{
					Thread.Sleep(3000);
					ExecuteJob(json);
					_Times--;
				}
			}
			catch (Exception ex)
			{
				_Logger.LogWarning(ex.Message, ex.StackTrace);
			}
		}

	}
}
