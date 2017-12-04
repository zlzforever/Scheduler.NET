using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using Scheduler.NET.Core.Scheduler;
using Scheduler.NET.Core.Utils;

namespace Scheduler.NET.Core.Controllers
{
	public class JobController : Controller
	{
		private readonly IJobManager _jobManager;
		private readonly static ILogger Logger = LogCenter.GetLogger();

		public JobController(IJobManager jobManager)
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
				if (value.Cron == "* * * * 2999")
				{
					return Ok();
				}
				var result = _jobManager.AddOrUpdate(value);

				if (string.IsNullOrEmpty(result))
				{
					return NoContent();
				}
				else
				{
					return new JsonResult(result);
				}
			}
			else
			{
				Logger.Error($"Error parameters: {JsonConvert.SerializeObject(value)}.");
				return BadRequest();
			}
		}

		[HttpPost]
		public IActionResult Remove(string jobId)
		{
			_jobManager.Remove(jobId);
			return Ok();
		}

		[HttpPost]
		public IActionResult Trigger(string jobId)
		{
			_jobManager.Trigger(jobId);
			return Ok();
		}
	}
}
