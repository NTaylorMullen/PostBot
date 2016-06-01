using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PostBot.Configuration;

namespace PostBot.Slack
{
    public class SlackMessageClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SlackConfiguration _configuration;

        public SlackMessageClient(SlackConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public void Post(SlackMessage message)
        {
            message.Channel = message.Channel ?? _configuration.PostChannel;
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            int retryCount = 0;

            while (retryCount++ < 5)
            {
                try
                {
                    var result = _httpClient.PostAsync(_configuration.WebHookUrl, content).Result;

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        break;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch
                {
                    // Ignore so we can retry
                }
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
