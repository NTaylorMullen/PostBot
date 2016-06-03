using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostBot.Slack.ApiResponses
{
    public class SlackMessageListResponse
    {
        public IEnumerable<SlackMessageResponse> Messages { get; set; }
    }
}
