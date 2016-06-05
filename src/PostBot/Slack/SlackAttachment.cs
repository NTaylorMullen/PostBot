using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PostBot.Slack
{
    public class SlackAttachment
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("mrkdwn_in")]
        public IEnumerable<string> MarkdownIn { get; } = new[] { "text" };

        [JsonProperty("color")]
        public string HexSidelineColor { get; set; }

        [JsonProperty("ts")]
        [JsonConverter(typeof(DateTimeOffsetToLongConverter))]
        public DateTimeOffset TimeStamp { get; set; }

        [JsonProperty("footer")]
        public string Footer { get; set; }
    }
}
