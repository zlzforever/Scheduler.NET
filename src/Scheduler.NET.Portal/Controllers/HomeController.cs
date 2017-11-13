using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Portal.Models;
using Scheduler.NET.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.Options;
using Scheduler.NET.Core.Scheduler;

namespace Scheduler.NET.Portal.Controllers
{
	public class HomeController : Controller
	{
		public IJobManager _JobManager { get; set; }

		public HomeController(IJobManager _jobManager)
		{
			_JobManager = _jobManager;
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
