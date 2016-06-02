using System;

namespace PostBot.Configuration
{
    public class SlackConfiguration
    {
        public string PostChannel { get; set; }

        public Uri WebHookUrl { get; set; }

        public int MessageBufferSize { get; set; }

        public string ApiToken { get; set; }

        public Uri SlackApiUrl { get; set; }
    }
}
