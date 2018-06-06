using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduler.NET.Portal.Models;

namespace Scheduler.NET.Portal.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger _logger;

		public HomeController(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger(GetType());
		}

		public IActionResult Index()
		{
			return Redirect("/hangfire");
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
