using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scheduler.NET
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum StorageType
	{
		Memory = 0,
		SqlServer = 1,
		MySql = 2,
		Redis = 3
	}
}
