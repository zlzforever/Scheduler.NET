using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;
using Scheduler.NET.Core.Domain;
using Microsoft.Extensions.Logging;
using Hangfire;

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
			//_JobManager.EnqueueHFJob(null);
			var its = JobStorage.Current.GetMonitoringApi().Queues();
			var it1 = JobStorage.Current.GetMonitoringApi().ProcessingCount();
			var it2 = JobStorage.Current.GetMonitoringApi().ProcessingJobs(1,100);
			var it3 = JobStorage.Current.GetMonitoringApi().ScheduledCount();
			var it4 = JobStorage.Current.GetMonitoringApi().ScheduledJobs(1, 100);
			var it5 = JobStorage.Current.GetMonitoringApi().SucceededListCount();
			var it6 = JobStorage.Current.GetMonitoringApi().SucceededJobs(1,100);

			var ss1 = JobStorage.Current.GetMonitoringApi().Servers();
			var ss2 = JobStorage.Current.GetMonitoringApi().FetchedCount("DEFAULT");
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
					_JobManager.AddOrUpdateHFJob(value);
				}
				return Messager.GetOKMessage("succeed.");
			}
			catch (Exception ex)
			{
				_Logger.LogWarning(ex.Message);
				return Messager.GetFailMessage("filed.");
			}
		}

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		// PUT: api/Task/5
		[HttpPut("{id}")]
		public void Modify(int id, [FromBody]string value)
		{

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
}
