using Jil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;

namespace Scheduler.NET.Core.Controllers
{
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
		public IActionResult Add([FromBody]CallbackJob value)
		{
			if (ModelState.IsValid)
			{
				// 这个时间永远不会触发, 表示是手动触发项目
				if (value.Cron == "* * * * 2999")
				{
					return Success();
				}
				var result = _jobManager.Add(value);

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
				throw new SchedulerException($"Error parameters: {JSON.Serialize(value)}.");
			}
		}

		[HttpPut]
		public IActionResult Update(string id, [FromBody]CallbackJob value)
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
					_jobManager.Update(id, value);
					return Success();
				}
			}
			else
			{
				throw new SchedulerException($"Error parameters: {JSON.Serialize(value)}.");
			}
		}

		[HttpDelete]
		public IActionResult Remove(string jobId)
		{
			_jobManager.Remove(jobId);
			return Success();
		}

		[HttpPost]
		public IActionResult Trigger(string jobId)
		{
			_jobManager.Trigger(jobId);
			return Success();
		}

		public IActionResult ThrowException()
		{
			throw new SchedulerException("hello");
		}
	}
}
