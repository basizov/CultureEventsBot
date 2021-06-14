using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CultureEventsBot.API.Core
{
    public static class HttpWork<T>
    {
        public static async Task<T>	SendRequestAsync(HttpMethod httpMethod, string url, IHttpClientFactory httpClient)
		{
			var request = new HttpRequestMessage(httpMethod, url);
			var clientHttp = httpClient.CreateClient();
			var responseJson = await clientHttp.SendAsync(request);
			T	response = default(T);

			if (responseJson.IsSuccessStatusCode)
				response = await responseJson.Content.ReadFromJsonAsync<T>();
			return(response);
		}
		public static async Task<T>	SendRequestWithHeadersAsync(HttpMethod httpMethod, string url, IHttpClientFactory httpClient, IDictionary<string, string> headers)
		{
			var request = new HttpRequestMessage(httpMethod, url);

			foreach (var header in headers)
				request.Headers.Add(header.Key, header.Value);
			var clientHttp = httpClient.CreateClient();
			var responseJson = await clientHttp.SendAsync(request);
			T	response = default(T);

			if (responseJson.IsSuccessStatusCode)
				response = await responseJson.Content.ReadFromJsonAsync<T>();
			return(response);
		}
    }
}