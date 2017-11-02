using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Propertities
{
	public class RestConnection
	{
		private readonly RestProperties rp;

		internal RestConnection(RestProperties restProperties)
		{
			rp = restProperties;
		}

		internal string SendToApi()
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(rp.Url);
			webRequest.ContentType = rp.ContentType;
			webRequest.Method = rp.Method.ToString();

			if (webRequest.Method == WebRequestMethods.Http.Post)
			{
				if (!string.IsNullOrEmpty(rp.PostData))
				{
					var postBytes = Encoding.UTF8.GetBytes(rp.PostData);
					webRequest.ContentLength = postBytes.Length;

					using (var dataStream = webRequest.GetRequestStream())
					{
						dataStream.Write(postBytes, 0, postBytes.Length);
					}
				}
			}
			try
			{
				using (var httpResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					using (var responseStream = httpResponse.GetResponseStream())
					{
						if (responseStream != null)
						{
							using (var streamReader = new StreamReader(responseStream))
							{
								return streamReader.ReadToEnd();
							}
						}
					}
				}
			}
			catch (WebException err)
			{
			}
			return string.Empty;
		}

	}
}
