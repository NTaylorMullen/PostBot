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
        private const string ListMessagesApi = "channels.history";
        private const string DeleteMessageApi = "chat.delete";
        private readonly SlackConfiguration _configuration;
        private readonly ResilientHttpClient _httpClient;
        private readonly ILogger _logger;
        private string _channelId;

        public SlackApiClient(ResilientHttpClient httpClient, ILogger logger, SlackConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;

            InitializeChannelId(_configuration.PostChannel);
        }


        public void Delete(SlackMessage message)
        {
            var messageTimeStamp = FindMessageTimeStamp(message);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private int FindMessageTimeStamp(SlackMessage message)
        {
            var attachmentTimeStamp = message.Attachments.First().TimeStamp;
            var unixTimeStamp = attachmentTimeStamp.ToUnixTimeSeconds();
            var queryParameters = new Dictionary<string, string>();
            queryParameters["count"] = (2 * _configuration.MessageBufferSize).ToString();
            queryParameters["oldest"] = (unixTimeStamp - 1).ToString();

            return 0;
        }

        private void InitializeChannelId(string channelName)
        {
            var authorizedQuery = GetAuthorizedQuery();
            var listChannelsUri = new Uri(_configuration.SlackApiUrl + ListChannelsApi + authorizedQuery);
            var result = _httpClient.PostJsonWithRetry(listChannelsUri);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            var channelList = JsonConvert.DeserializeObject<SlackChannelList>(resultContent);
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
