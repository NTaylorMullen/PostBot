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
        private readonly SlackConfiguration _configuration;
        private readonly ResilientHttpClient _httpClient;

        public SlackMessageClient(ResilientHttpClient httpClient, SlackConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public void Post(SlackMessage message)
        {
            message.Channel = message.Channel ?? $"#{_configuration.PostChannel}";
            var messageGuid = Guid.NewGuid().ToString();

            // Mark each attachment with a message based GUID so we can find them later
            foreach (var attachment in message.Attachments)
            {
                attachment.Fallback = messageGuid;
            }

            _httpClient.PostJsonWithRetry(_configuration.WebHookUrl, message);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
