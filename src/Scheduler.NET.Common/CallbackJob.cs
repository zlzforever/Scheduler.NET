using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Scheduler.NET.Common
{
	public sealed class CallbackJob : IJob
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
		/// 任务名称 
		/// </summary>
		[Required]
		[StringLength(100)]
		public string ClassName { get; set; }

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

		[Required]
		[Url]
		public string Url { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public HttpMethod Method { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
