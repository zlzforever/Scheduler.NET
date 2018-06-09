using System.Net;
using System.Net.Http;
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

		public static async Task<HttpResponseMessage> Get(string url)
		{
			return await Client.GetAsync(url);
		}
	}
}
