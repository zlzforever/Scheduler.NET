using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Scheduler.NET.Common
{
	/// <inheritdoc />
	// ReSharper disable once ClassNeverInstantiated.Global
	public class CallbackJob : Job
	{
		[Required]
		[Url]
		public virtual string Url { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public virtual HttpMethod Method { get; set; }
	}
}
