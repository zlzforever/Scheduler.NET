using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core
{
	public class SchedulerConfig
	{
		public static readonly String SqlServerConnectString = @"Server=DESKTOP-AEQD6I9;User ID = sa; Password=111111;database=HangFire;";

		//public static readonly String RedisCachingConnectionString = @"127.0.0.1:6379,serviceName=DotnetSpider,keepAlive=8,allowAdmin=True,connectTimeout=10000,abortConnect=True,connectRetry=20";
		public static readonly String RedisCachingConnectionString = @"127.0.0.1:6379";
	}
}
