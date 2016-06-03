using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
using PostBot.Configuration;

namespace PostBot.Slack
{
    public class SlackClient : IDisposable
    {
        private const string MessageBufferFileName = "messagebuffer.json";
        private readonly SlackApiClient _apiClient;
        private readonly SlackConfiguration _configuration;
        private readonly SlackMessageClient _messageClient;
        private Queue<SlackMessage> _messageBuffer;
        private readonly object _postLock = new object();

        public SlackClient(SlackConfiguration configuration)
        {
            _configuration = configuration;
            InitializeMessageBufferFromDisk();

            var httpClient = new ResilientHttpClient();
            _messageClient = new SlackMessageClient(httpClient, configuration);
            var logger = new ConsoleLogger("SlackClient", (name, level) => true, includeScopes: true);
            _apiClient = new SlackApiClient(httpClient, logger, configuration);
        }

        public void Post(SlackMessage message)
        {
            lock (_postLock)
            {
                if (_messageBuffer.Count == _configuration.MessageBufferSize)
                {
                    var oldMessage = _messageBuffer.Dequeue();

                    // We delete an old message prior to posting a new message to not exhaust the Slack history limit.
                    _apiClient.Delete(oldMessage);
                }

                _messageBuffer.Enqueue(message);

                _messageClient.Post(message);

                SaveMessageBufferToDisk();
            }
        }

        public void Dispose()
        {
            _messageClient.Dispose();
            _apiClient.Dispose();
        }

        private void InitializeMessageBufferFromDisk()
        {
            if (!File.Exists(MessageBufferFileName))
            {
                _messageBuffer = new Queue<SlackMessage>(_configuration.MessageBufferSize);
                return;
            }

            using (var file = File.OpenText(MessageBufferFileName))
            using (var reader = new JsonTextReader(file))
            {
                var serializer = new JsonSerializer();
                _messageBuffer = serializer.Deserialize<Queue<SlackMessage>>(reader) ?? new Queue<SlackMessage>(_configuration.MessageBufferSize);
            }
        }

        private void SaveMessageBufferToDisk()
        {
            if (!File.Exists(MessageBufferFileName))
            {
                File.Create(MessageBufferFileName).Dispose();
            }

            using (var fileStream = File.Open(MessageBufferFileName, FileMode.Truncate))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;

                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, _messageBuffer);
            }
        }
    }
}
