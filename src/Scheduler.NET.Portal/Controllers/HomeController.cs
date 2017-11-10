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

namespace Scheduler.NET.Portal.Controllers
{
	public class HomeController : Controller
	{
		public HomeController()
		{
		}

		public IActionResult Index()
        {
            return View();
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
