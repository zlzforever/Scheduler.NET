using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;
using Microsoft.Extensions.Logging;
using Hangfire;
using DotnetSpider.Enterprise.Core.Utils;
using System.Net.Http;
using Newtonsoft.Json;

namespace Scheduler.NET.Portal.Controllers
{
	public class TaskController : Controller
	{
		private readonly IJobManager _jobManager;

		public TaskController(IJobManager jobManager)
		{
			_jobManager = jobManager;
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		[HttpPost]
		public IActionResult AddOrUpdate([FromBody]Job value)
		{
			if (ModelState.IsValid)
			{
				return new JsonResult(_jobManager.AddOrUpdate(value));
			}
			return BadRequest();
		}

		[HttpPost]
		public IActionResult Remove(string jobId)
		{
			if (ModelState.IsValid)
			{
				_jobManager.Remove(jobId);
			}
			return Ok();
		}
	}
}
