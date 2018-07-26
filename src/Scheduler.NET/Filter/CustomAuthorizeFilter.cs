using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Scheduler.NET.Filter
{
	public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			return true;
		}
	}
}
