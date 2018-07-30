using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.JobManager;
using Scheduler.NET.Common;

namespace Scheduler.NET.Controllers
{
	public abstract class BaseController<T> : Controller where T : IJob
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
				throw new SchedulerNetException("Auth dined.");
			}

			base.OnActionExecuting(context);
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value">任务</param>
		[HttpPost]
		public IActionResult Create([FromBody]T value)
		{
			if (ModelState.IsValid)
			{
				string result = null;
				if (_schedulerOptions.IgnoreCrons != null && _schedulerOptions.IgnoreCrons.Contains(value.Cron.Trim()))
				{
					Logger.LogInformation($"Ignore job {JsonConvert.SerializeObject(value)}.");
				}
				else
				{
					result = _jobManager.Create(value);
				}
				return Success(result);
			}
			else
			{
				throw new SchedulerNetException($"[CREATE] {JsonConvert.SerializeObject(value)} is not valid.");
			}
		}

		[HttpPut]
		public IActionResult Update([FromBody]T value)
		{
			if (ModelState.IsValid)
			{
				if (_schedulerOptions.IgnoreCrons != null && _schedulerOptions.IgnoreCrons.Contains(value.Cron.Trim()))
				{
					Logger.LogInformation($"Ignore job {JsonConvert.SerializeObject(value)}.");
					_jobManager.Delete(value.Id);
				}
				else
				{
					_jobManager.Update(value);
				}
				return Success();
			}
			else
			{
				throw new SchedulerNetException($"[UPDATE] {JsonConvert.SerializeObject(value)} is not valid.");
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id)
		{
			_jobManager.Delete(id);
			return Success();
		}

		[HttpGet("{id}")]
		public IActionResult Fire(string id)
		{
			_jobManager.Fire(id);
			return Success();
		}

		protected IActionResult Success(dynamic data = null, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Success = true, Data = data, Message = message });
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
