using System;

namespace Scheduler.NET.Common
{
	public class JobContext
	{
		/// <summary>
		/// 任务标识
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 任务分组
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// 任务名称 
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// 定时表达式
		/// </summary>
		public string Cron { get; set; }

		/// <summary>
		/// 任务描述、内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 触发时间
		/// </summary>
		public DateTime FireTime { get; set; }
	}
}
