using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CultureEventsBot.API.Core
{
    public static class HttpWork<T>
    {
        public static async Task<T>	SendRequestAsync(string url, IHttpClientFactory httpClient)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, url);
			var clientHttp = httpClient.CreateClient();
			var responseJson = await clientHttp.SendAsync(request);
			T	response = default(T);

			if (responseJson.IsSuccessStatusCode)
				response = await responseJson.Content.ReadFromJsonAsync<T>();
			return(response);
		}
    }
}