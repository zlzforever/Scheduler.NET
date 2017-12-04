using NLog;
using NLog.Config;
using System;
using System.IO;

namespace Scheduler.NET.Core.Utils
{
	public static class LogCenter
	{
		static LogCenter()
		{
			string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");

			if (!File.Exists(nlogConfigPath))
			{
				throw new SchedulerException("nlog.config unfound.");
			}
			XmlLoggingConfiguration configuration = new XmlLoggingConfiguration(nlogConfigPath);

			configuration.Install(new InstallationContext());
			LogManager.Configuration = configuration;
		}

		public static ILogger GetLogger()
		{
			return LogManager.GetLogger("Scheduler.NET");
		}
	}
}
