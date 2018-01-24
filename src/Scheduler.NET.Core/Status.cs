using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.NET.Core
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		Sucess,
		Error,
		Failed
	}
}
