using DotnetSpider.Enterprise.Core.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Scheduler.NET.Core.Scheduler
{

	/// <summary>
	/// 
	/// </summary>
	public class RecurringJobService
	{

		private readonly ILogger<RecurringJobService> _Logger;

		public RecurringJobService(ILogger<RecurringJobService> _logger)
		{
			this._Logger = _logger;
		}

		public void ExecuteJob(params String[] arguments)
		{
			try
			{
				string url = String.Format("{0}?taskId={1}&token={2}", arguments[0], arguments[1], arguments[2]);
				HttpUtil.RequestUrl<String>(url, HttpMethod.Post);
			}
			catch (Exception ex)
			{
				_Logger.LogWarning(ex.Message, ex.StackTrace);
			}
		}

	}
}
