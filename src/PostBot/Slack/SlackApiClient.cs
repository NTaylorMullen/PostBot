using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostBot.Configuration;
using PostBot.Slack.ApiResponses;

namespace PostBot.Slack
{
    public class SlackApiClient : IDisposable
    {
        private const string ListChannelsApi = "channels.list";
        private const string ChannelMessageHistoryApi = "channels.history";
        private const string DeleteMessageApi = "chat.delete";
        private readonly SlackConfiguration _configuration;
        private readonly ResilientHttpClient _httpClient;
        private readonly ILogger _logger;
        private string _channelId;
        private SlackMessageListResponse _cachedMessageListResponse;

        public SlackApiClient(ResilientHttpClient httpClient, ILogger logger, SlackConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;

            InitializeChannelId(_configuration.PostChannel);
        }


        public void Delete(SlackMessage message)
        {
            var messageToDeleteTimeStamp = FindMessageToDeleteTimeStamp(message);

            if (messageToDeleteTimeStamp == null)
            {
                System.Diagnostics.Debugger.Launch();
                var test = FindMessageToDeleteTimeStamp(message);
                _logger.LogError("Could not locate message for deletion: " + message.Attachments.First().Text);
                return;
            }

            var queryParameters = new Dictionary<string, string>
            {
                ["as_user"] = "true",
                ["channel"] = _channelId,
                ["ts"] = messageToDeleteTimeStamp
            };

            var authorizedQuery = GetAuthorizedQuery(queryParameters);
            var deleteMessageUri = new Uri(_configuration.SlackApiUrl + DeleteMessageApi + authorizedQuery);
            _httpClient.PostJsonWithRetry(deleteMessageUri);

            _logger.LogInformation("Deleting message: " + message.Attachments?.First().Text);
            _logger.LogInformation("Uri to delete message: " + deleteMessageUri.ToString());
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private string FindMessageToDeleteTimeStamp(SlackMessage message)
        {
            var attachmentTimeStamp = message.Attachments.Last().TimeStamp;
            var attachmentToFind = message.Attachments.Last();
            var messageToDeletesTimeStamp = ExtractMessageToDeleteTimeStamp(attachmentToFind, _cachedMessageListResponse);

            if (messageToDeletesTimeStamp != null)
            {
                return messageToDeletesTimeStamp;
            }

            var unixTimeStamp = attachmentTimeStamp.ToUnixTimeSeconds();
            var queryParameters = new Dictionary<string, string>
            {
                // Download 2x the buffer in-case there was chatter in the channel we're posting to.
                ["count"] = (2 * _configuration.MessageBufferSize).ToString(),

                // Oldest should be 1 second less than the timestamp we're looking for to ensure we captuer it.
                ["oldest"] = (unixTimeStamp - 10).ToString(),
                ["inclusive"] = "1",
                ["channel"] = _channelId
            };

            var authorizedQuery = GetAuthorizedQuery(queryParameters);
            var channelMessageHistoryUri = new Uri(_configuration.SlackApiUrl + ChannelMessageHistoryApi + authorizedQuery);
            var result = _httpClient.PostJsonWithRetry(channelMessageHistoryUri);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            _cachedMessageListResponse = JsonConvert.DeserializeObject<SlackMessageListResponse>(resultContent);

            messageToDeletesTimeStamp = ExtractMessageToDeleteTimeStamp(attachmentToFind, _cachedMessageListResponse);

            return messageToDeletesTimeStamp;
        }

        private string ExtractMessageToDeleteTimeStamp(
            SlackAttachment originalAttachment,
            SlackMessageListResponse messageListResponse)
        {
            if (messageListResponse == null)
            {
                return null;
            }

            string messageToDeletesTimeStamp = null;
            foreach (var responseMessage in messageListResponse.Messages)
            {
                if (responseMessage.Attachments?.Any(attachment => attachment.TimeStamp == originalAttachment.TimeStamp) == true &&
                    responseMessage.Attachments?.Any(attachment => string.Equals(attachment.Fallback, originalAttachment.Fallback, StringComparison.Ordinal)) == true)
                {
                    messageToDeletesTimeStamp = responseMessage.TimeStamp;
                }
            }

            return messageToDeletesTimeStamp;
        }

        private void InitializeChannelId(string channelName)
        {
            var authorizedQuery = GetAuthorizedQuery();
            var listChannelsUri = new Uri(_configuration.SlackApiUrl + ListChannelsApi + authorizedQuery);
            var result = _httpClient.PostJsonWithRetry(listChannelsUri);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            var channelList = JsonConvert.DeserializeObject<SlackChannelListResponse>(resultContent);
            var postChannel = channelList.Channels.Single(
                channel => string.Equals(channel.Name, _configuration.PostChannel, StringComparison.Ordinal));
            _channelId = postChannel.Id;

            _logger.LogInformation($"Resolved channel '{_configuration.PostChannel}' to have ID '{_channelId}'.");
        }

        private string GetAuthorizedQuery(Dictionary<string, string> queryParameters = null)
        {
            var queryStringBuilder = new QueryBuilder(queryParameters ?? new Dictionary<string, string>());
            queryStringBuilder.Add("token", _configuration.ApiToken);

            return queryStringBuilder.ToString();
        }
    }
}
