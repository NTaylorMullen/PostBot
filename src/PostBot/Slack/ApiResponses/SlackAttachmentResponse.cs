using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PostBot.Slack.ApiResponses
{
    public class SlackAttachmentResponse
    {
        [JsonProperty("ts")]
        [JsonConverter(typeof(DateTimeOffsetToLongConverter))]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
