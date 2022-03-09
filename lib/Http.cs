using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AdminHelper.lib
{
    public static class Http
    {
        private static readonly int timeoutSec = 90;
        private static readonly HttpClient client = new HttpClient();
        public static async Task<byte[]> Request(
            string method,
            string uri,
            string body = "",
            string contentType = "application/json")
        {
            try
            {
                client.BaseAddress = new Uri(uri);
                client.Timeout = new TimeSpan(0, 0, timeoutSec);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                client.DefaultRequestHeaders.Add("User-Agent", "AdminHelper");

                HttpResponseMessage response;
                switch (new HttpMethod(method).ToString().ToUpper())
                {
                    case "GET":
                        {
                            response = await client.GetAsync(uri);
                        }
                        break;
                    case "POST":
                        {
                            HttpContent Body = new StringContent(body);
                            Body.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                            response = await client.PostAsync(uri, Body);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                throw new Exception($"Http.Request {method} {uri} is failed");
            }
        }
    }
}