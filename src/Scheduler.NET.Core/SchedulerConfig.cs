using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core
{
	public class SchedulerConfig
	{
		public SqlConfig SqlConfig { get; set; }
		public RedisConfig RedisConfig { get; set; }

	}

	public class RedisConfig
	{
		public bool Enable { get; set; }

		public String ConnectionString { get; set; }
	}

	public class SqlConfig
	{
		public bool Enable { get; set; }

		public String ConnectionString { get; set; }
	}
}
