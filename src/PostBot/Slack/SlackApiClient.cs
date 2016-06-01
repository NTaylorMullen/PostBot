using System;
using System.Net.Http;
using PostBot.Configuration;

namespace PostBot.Slack
{
    public class SlackApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public SlackApiClient(SlackConfiguration configuration)
        {
            _httpClient = new HttpClient();
        }

        public void Delete(SlackMessage message)
        {
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
