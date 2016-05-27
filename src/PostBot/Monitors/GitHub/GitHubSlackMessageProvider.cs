using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using PostBot.Configuration;
using PostBot.Posters;

namespace PostBot.Monitors.GitHub
{
    public class GitHubSlackMessageProvider
    {
        private readonly GithubConfiguration _configuration;

        public GitHubSlackMessageProvider(GithubConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SlackMessage GetMessage(IEnumerable<Activity> activities)
        {
            var attachments = new List<SlackAttachment>();

            foreach (var activity in activities)
            {
                var attachment = new SlackAttachment
                {
                    Footer = "GitHub",
                    TimeStamp = activity.CreatedAt,
                    HexSidelineColor = "#000000",
                    Text = ""
                };
            }

            var message = new SlackMessage
            {
                UserName = "GitHub",
                IconUrl = _configuration.IconUrl,
                Attachments = attachments
            };

            return message;
        }
    }
}
