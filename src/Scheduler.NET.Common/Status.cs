using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scheduler.NET.Common
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		Success,
		Error
	}
}
