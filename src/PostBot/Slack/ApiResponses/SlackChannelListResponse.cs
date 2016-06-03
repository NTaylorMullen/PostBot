using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostBot.Slack.ApiResponses
{
    public class SlackChannelListResponse
    {
        public IEnumerable<SlackChannelResponse> Channels { get; set; }
    }
}
