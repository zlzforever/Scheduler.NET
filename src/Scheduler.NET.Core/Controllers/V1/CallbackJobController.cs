using Hangfire.Annotations;
using Jil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.Controllers.V1
{
	[Route("api/v1.0/[controller]")]
	public class CallbackJobController : BaseController
	{
		private readonly IJobManager<CallbackJob> _jobManager;
		private readonly ILogger _logger;

		public CallbackJobController(IJobManager<CallbackJob> jobManager, ILoggerFactory loggerFactory, ISchedulerConfiguration configuration) : base(configuration)
		{
			_jobManager = jobManager;
			_logger = loggerFactory.CreateLogger<CallbackJobController>();
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		[HttpPost]
		public IActionResult Create([FromBody]CallbackJob value)
		{
			if (ModelState.IsValid)
			{
				// 这个时间永远不会触发, 表示是手动触发项目
				if (value.Cron == "* * * * 2999")
				{
					return Success();
				}
				var result = _jobManager.Create(value);

				if (string.IsNullOrEmpty(result))
				{
					return Failed("Add job failed.");
				}
				else
				{
					return Success(result);
				}
			}
			else
			{
				throw new SchedulerException($"Error parameters: {GetModelStateError()}.");
			}
		}

		[HttpPut]
		public IActionResult Update([FromBody]CallbackJob value)
		{
			if (ModelState.IsValid)
			{
				// 这个时间永远不会触发, 表示是手动触发项目
				if (value.Cron == "* * * * 2999")
				{
					return Success();
				}
				else
				{
					_jobManager.Update(value);
					return Success();
				}
			}
			else
			{
				throw new SchedulerException($"Error parameters: {GetModelStateError()}.");
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id)
		{
			_jobManager.Delete(id);
			return Success();
		}

		[HttpGet("{id}")]
		public IActionResult Trigger(string id)
		{
			_jobManager.Trigger(id);
			return Success();
		}
	}
}
