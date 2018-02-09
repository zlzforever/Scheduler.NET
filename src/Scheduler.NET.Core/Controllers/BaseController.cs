using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;
using System.Linq;

namespace Scheduler.NET.Core.Controllers
{
	public abstract class BaseController<T> : Controller where T : BaseJob
	{
		private readonly IJobManager<T> _jobManager;
		private readonly ISchedulerConfiguration _schedulerConfiguration;
		protected readonly ILogger Logger;

		protected BaseController(IJobManager<T> jobManager, ILoggerFactory loggerFactory, ISchedulerConfiguration configuration)
		{
			_schedulerConfiguration = configuration;
			_jobManager = jobManager;
			Logger = loggerFactory.CreateLogger(GetType());
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!IsAuth())
			{
				throw new SchedulerException("Auth dined.");
			}

			base.OnActionExecuting(context);
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		[HttpPost]
		public IActionResult Create([FromBody]T value)
		{
			if (ModelState.IsValid)
			{
				// 这个时间永远不会触发, 表示是手动触发项目
				if (value.Cron.Contains("2999"))
				{
					return Success();
				}
				var result = _jobManager.Create(value);
				return Success(result);
			}
			else
			{
				throw new SchedulerException($"Error parameters: {GetModelStateError()}.");
			}
		}

		[HttpPut]
		public IActionResult Update([FromBody]T value)
		{
			if (ModelState.IsValid)
			{
				// 这个时间永远不会触发, 表示是手动触发项目
				if (value.Cron.Contains("2999"))
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

		protected IActionResult Success()
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success });
		}

		protected IActionResult Success(object data, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success, Data = data, Message = message });
		}

		protected IActionResult Failed(string message = null)
		{
			return new JsonResult(new StandardResult { Code = 103, Status = Status.Failed, Message = message });
		}

		protected string GetModelStateError()
		{
			foreach (var item in ModelState.Values)
			{
				if (item.Errors.Count > 0)
				{
					return item.Errors[0].ErrorMessage;
				}
			}
			return "";
		}

		private bool IsAuth()
		{
			if (!_schedulerConfiguration.AuthorizeApi)
			{
				return true;
			}
			if (Request.Headers.ContainsKey(_schedulerConfiguration.TokenHeader))
			{
				var token = Request.Headers[_schedulerConfiguration.TokenHeader].ToString();
				return _schedulerConfiguration.Tokens.Contains(token);
			}
			else
			{
				return false;
			}
		}
	}
}
