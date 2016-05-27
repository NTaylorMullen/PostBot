using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PostBot.Slack
{
    public class SlackMessage
    {
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("icon_url")]
        public Uri IconUrl { get; set; }

        [JsonProperty("attachments")]
        public IEnumerable<SlackAttachment> Attachments { get; set; }
    }
}
