using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Scheduler.NET.Core.Scheduler
{
	[DataContract(Name = "job")]
	public class SpiderJob
	{
		[DataMember(Name = "id")]
		public String TaskId { get; set; }

		[DataMember(Name = "cron")]
		public String Cron { get; set; }

		[DataMember(Name = "callback")]
		public String CallBack { get; set; }

		[DataMember(Name = "token")]
		public String Token { get; set; }

		[DataMember(Name = "posttime")]
		public DateTime PostTime { get; set; }
	}

}
