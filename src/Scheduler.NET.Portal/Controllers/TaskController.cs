using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;
using Scheduler.NET.Core.Entities;
using Microsoft.Extensions.Logging;
using Hangfire;
using DotnetSpider.Enterprise.Core.Utils;
using System.Net.Http;
using Newtonsoft.Json;

namespace Scheduler.NET.Portal.Controllers
{

	[Produces("application/json")]
	[Route("api/Task")]
	public class TaskController : Controller
	{
		private readonly ILogger<TaskController> _Logger;

		private readonly IJobManager _JobManager;

		public TaskController(IJobManager _jobManager, ILogger<TaskController> _logger)
		{
			_JobManager = _jobManager;
			_Logger = _logger;
		}

		// GET: api/Task/5
		[HttpGet("{id}", Name = "Get")]
		public object Get(String id)
		{
			//var user = new User() { Name = "ssss" };
			//HttpUtil.PostUrl("http://localhost:54106/api/values?id=1123", JsonConvert.SerializeObject(user));
			//_JobManager.EnqueueHFJob(null);
			//var its = JobStorage.Current.GetMonitoringApi().Queues();
			//var it1 = JobStorage.Current.GetMonitoringApi().ProcessingCount();
			//var it2 = JobStorage.Current.GetMonitoringApi().ProcessingJobs(1,100);
			//var it3 = JobStorage.Current.GetMonitoringApi().ScheduledCount();
			//var it4 = JobStorage.Current.GetMonitoringApi().ScheduledJobs(1, 100);
			//var it5 = JobStorage.Current.GetMonitoringApi().SucceededListCount();
			//var it6 = JobStorage.Current.GetMonitoringApi().SucceededJobs(1,100);

			//var ss1 = JobStorage.Current.GetMonitoringApi().Servers();
			//var ss2 = JobStorage.Current.GetMonitoringApi().FetchedCount("DEFAULT");
			return "value";
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		// POST: api/Task
		[HttpPost]
		public Message Add([FromBody]SpiderJob value)
		{
			try
			{
				if (value != null)
				{
					if (string.IsNullOrEmpty(value.Cron))
					{
						_JobManager.RemoveHFJob(value.TaskId);
					}
					else
					{
						_JobManager.AddOrUpdateHFJob(value);
					}
				}
				return Messager.GetOKMessage("succeed.");
			}
			catch (Exception ex)
			{
				_Logger.LogWarning(ex.Message);
				return Messager.GetFailMessage("failed.");
			}
		}

		/// <summary>
		/// 删除任务
		/// </summary>
		/// <param name="id"></param>
		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Remove(int id)
		{

		}
	}

	public class User
	{
		public string Name { get; set; }
	}
}
