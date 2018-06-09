using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scheduler.NET.Core
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		Success,
		Error
	}
}
