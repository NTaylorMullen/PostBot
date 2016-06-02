using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Console;
using PostBot.Configuration;

namespace PostBot.Slack
{
    public class SlackClient : IDisposable
    {
        private readonly SlackApiClient _apiClient;
        private readonly SlackConfiguration _configuration;
        private readonly Queue<SlackMessage> _messageBuffer;
        private readonly SlackMessageClient _messageClient;

        public SlackClient(SlackConfiguration configuration)
        {
            _configuration = configuration;
            _messageBuffer = new Queue<SlackMessage>(configuration.MessageBufferSize);

            var httpClient = new ResilientHttpClient();
            _messageClient = new SlackMessageClient(httpClient, configuration);
            var logger = new ConsoleLogger("SlackClient", (name, level) => true, includeScopes: true);
            _apiClient = new SlackApiClient(httpClient, logger, configuration);
        }

        public void Post(SlackMessage message)
        {
            if (_messageBuffer.Count == _configuration.MessageBufferSize)
            {
                var oldMessage = _messageBuffer.Dequeue();

                // We delete an old message prior to posting a new message to not exhaust the Slack history limit.
                _apiClient.Delete(oldMessage);
            }

            _messageBuffer.Enqueue(message);

            _messageClient.Post(message);
        }

        public void Dispose()
        {
            _messageClient.Dispose();
            _apiClient.Dispose();
        }
    }
}
