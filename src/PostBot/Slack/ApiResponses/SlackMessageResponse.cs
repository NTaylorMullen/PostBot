using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PostBot.Slack.ApiResponses
{
    public class SlackMessageResponse
    {
        [JsonProperty("ts")]
        public string TimeStamp { get; set; }

        public IEnumerable<SlackAttachmentResponse> Attachments { get; set; }
    }
}
