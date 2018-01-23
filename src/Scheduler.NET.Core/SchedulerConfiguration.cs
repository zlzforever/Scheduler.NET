using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core
{
	public class SchedulerConfiguration : ISchedulerConfiguration
	{
		public const string DefaultSettingKey = "SchedulerNET";

		public bool AuthorizeApi { get; set; }
		public string[] Tokens { get; set; }
		public string HangfireStorageType { get; set; }
		public string HangfireConnectionString { get; set; }
		public string Host { get; set; }
		public string TokenHeader { get; set; }
	}
}
