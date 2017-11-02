using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Propertities
{
	public class RestProperties
	{
		public RestProperties()
		{
			Url = String.Empty;
			Method = HttpMethod.Post;
			PostData = String.Empty;
			ContentType = HttpContentTypes.ApplicationJson;
		}

		public HttpMethod Method { get; set; }

		public String Url { get; set; }

		public String PostData { get; set; }

		public String ContentType { get; set; }
	}
}
