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
		public virtual string Id { get; set; }

		/// <summary>
		/// 任务分组
		/// </summary>
		[StringLength(100)]
		public virtual string Group { get; set; }

		/// <summary>
		/// 任务名称 
		/// </summary>
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		/// <summary>
		/// 定时表达式
		/// </summary>
		[Required]
		[StringLength(20)]
		public virtual string Cron { get; set; }

		/// <summary>
		/// 任务描述
		/// </summary>
		[StringLength(500)]
		public virtual string Detail { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnable { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
