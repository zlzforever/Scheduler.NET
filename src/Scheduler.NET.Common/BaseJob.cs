using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Scheduler.NET.Common
{
	/// <summary>
	/// 任务抽象
	/// </summary>
	public abstract class BaseJob : IJob
	{
		/// <summary>
		/// 任务标识
		/// </summary>
		[StringLength(32)]
		public string Id { get; set; }

		/// <summary>
		/// 任务分组
		/// </summary>
		[StringLength(100)]
		public string Group { get; set; }

		/// <summary>
		/// 任务名称: 完整的类名
		/// </summary>
		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		/// <summary>
		/// 定时表达式
		/// </summary>
		[Required]
		[StringLength(20)]
		public string Cron { get; set; }

		/// <summary>
		/// 任务描述、内容
		/// </summary>
		[StringLength(500)]
		public string Content { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
