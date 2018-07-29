using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler.NET.Common;
using System.Text;

namespace Scheduler.NET
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
			var info = JsonConvert.SerializeObject(context.Exception is SchedulerNetException ? new StandardResult { Code = 101, Message = context.Exception.Message, Success = false } : new StandardResult { Code = 102, Message = "internal error", Success = false });

			_logger.LogError(context.Exception.ToString());

			var bytes = Encoding.UTF8.GetBytes(info);
			context.ExceptionHandled = true;
			context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
			context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
		}
	}
}
