using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Core.JobManager;
using Scheduler.NET.Core.JobManager.Job;
using System.Text;

namespace Scheduler.NET.Core.Controllers
{
	public abstract class BaseController<T> : Controller where T : BaseJob
	{
		private readonly IJobManager<T> _jobManager;
		private readonly ISchedulerOptions _schedulerOptions;
		protected readonly ILogger Logger;

		protected BaseController(IJobManager<T> jobManager, ILoggerFactory loggerFactory, ISchedulerOptions options)
		{
			_schedulerOptions = options;
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
				if (_schedulerOptions.IgnoreCrons.Contains(value.Cron.Trim()))
				{
					Logger.LogInformation($"Ignore job: {JsonConvert.SerializeObject(value)}.");
					return Success();
				}
				else
				{
					var result = _jobManager.Create(value);
					return Success(result);
				}
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
				if (_schedulerOptions.IgnoreCrons.Contains(value.Cron.Trim()))
				{
					Logger.LogInformation($"Ignore job: {JsonConvert.SerializeObject(value)}.");
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

		protected IActionResult Success(dynamic data, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success, Data = data, Message = message });
		}

		protected string GetModelStateError()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var item in ModelState.Values)
			{
				builder.Append(string.Join(", ", item.Errors));
			}
			return builder.ToString();
		}

		private bool IsAuth()
		{
			if (!_schedulerOptions.UseToken)
			{
				return true;
			}
			if (Request.Headers.ContainsKey(_schedulerOptions.TokenHeader))
			{
				var token = Request.Headers[_schedulerOptions.TokenHeader].ToString();
				return _schedulerOptions.Tokens.Contains(token);
			}
			else
			{
				return false;
			}
		}
	}
}
