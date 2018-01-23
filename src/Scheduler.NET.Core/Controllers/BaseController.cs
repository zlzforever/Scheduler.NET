using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Scheduler.NET.Core.Controllers
{
	public class BaseController : Controller
	{
		private readonly ISchedulerConfiguration _schedulerConfiguration;

		protected BaseController(ISchedulerConfiguration configuration)
		{
			_schedulerConfiguration = configuration;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!IsAuth())
			{
				throw new SchedulerException("Auth dined.");
			}

			base.OnActionExecuting(context);
		}

		public IActionResult Success()
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Sucess });
		}

		public IActionResult Success(object data, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Sucess, Data = data, Message = message });
		}

		public IActionResult Failed(string message = null)
		{
			return new JsonResult(new StandardResult { Code = 103, Status = Status.Failed, Message = message });
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
