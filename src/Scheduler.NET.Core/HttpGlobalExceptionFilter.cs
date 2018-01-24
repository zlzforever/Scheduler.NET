using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Scheduler.NET.Core
{

	public class HttpGlobalExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<HttpGlobalExceptionFilter> _logger;

		public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			context.HttpContext.Response.StatusCode = 206;
			string info;
			if (context.Exception is SchedulerException)
			{
				info = Jil.JSON.Serialize(new StandardResult { Code = 101, Message = context.Exception.Message, Status = Status.Error });

				_logger.LogError(context.Exception.ToString());

				if (context.Exception.InnerException != null)
				{
					_logger.LogError(context.Exception.InnerException.ToString());
				}
			}
			else
			{
				_logger.LogError(context.Exception.ToString());
				info = Jil.JSON.Serialize(new StandardResult { Code = 102, Message = "Internal error.", Status = Status.Error });
			}
			var bytes = Encoding.UTF8.GetBytes(info);
			context.ExceptionHandled = true;
			context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
			context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
		}
	}
}
