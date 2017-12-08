using System.Net;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Utils
{
	public static class HttpUtil
	{
		private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			UseCookies = false
		});

		public static HttpResponseMessage Post(string url, string data)
		{
			var postData = $"data={WebUtility.HtmlEncode(data)}";
			var content = new StringContent(postData, Encoding.UTF8, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
			return Client.PostAsync(url, content).Result;
		}
	}
}
