using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace PostBot.Slack
{
    public class ResilientHttpClient : HttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly int _maxRetryCount = 5;

        public ResilientHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public HttpResponseMessage PostJsonWithRetry(Uri requestUri)
        {
            return PostJsonWithRetry(requestUri, new object());
        }

        public HttpResponseMessage PostJsonWithRetry(Uri requestUri, object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int retryCount = 0;
            while (retryCount++ < _maxRetryCount)
            {
                try
                {
                    var result = _httpClient.PostAsync(requestUri, content).Result;

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return result;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch
                {
                    // Ignore so we can retry
                }
            }

            return null;
        }
    }
}
