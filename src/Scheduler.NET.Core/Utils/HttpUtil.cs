using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

		public static async Task<HttpResponseMessage> Post(string url, string data)
		{
			var content = new StringContent(data, Encoding.UTF8, HttpContentTypes.ApplicationJson);
			return await Client.PostAsync(url, content);
		}
	}
}
