using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.NET.Portal.Filter
{
	public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			return true;
		}
	}
}
