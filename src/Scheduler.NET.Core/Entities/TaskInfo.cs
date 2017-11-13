using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Entities
{

	/// <summary>
	/// 计划任务
	/// </summary>
	public class TaskInfo
	{
		public virtual string Name { get; set; }

		public virtual string Arguments { get; set; }

		public virtual string SpiderName { get; set; }

		public virtual int CountOfNodes { get; set; }

		public virtual string Cron { get; set; }

		public virtual bool IsEnabled { get; set; }

		public virtual string Version { get; set; }

		public virtual int ProjectId { get; set; }

	}
}
