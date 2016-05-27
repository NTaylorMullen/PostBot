using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Octokit;
using PostBot.Configuration;
using PostBot.Slack;

namespace PostBot.Monitors.GitHub
{
    public class GitHubSlackMessageProvider
    {
        private readonly GithubConfiguration _configuration;
        private readonly GitHubSlackAttachmentTextProvider _attachmentTextProvider;

        public GitHubSlackMessageProvider(ILogger logger, GithubConfiguration configuration)
        {
            _configuration = configuration;
            _attachmentTextProvider = new GitHubSlackAttachmentTextProvider(logger, configuration);
        }

        public bool TryGetMessage(IEnumerable<Activity> activities, out SlackMessage message)
        {
            var attachments = new List<SlackAttachment>();

            foreach (var activity in activities)
            {
                string attachmentText;
                if (!_attachmentTextProvider.TryGetAttachmentText(activity, out attachmentText))
                {
                    continue;
                }

                var attachment = new SlackAttachment
                {
                    Footer = "GitHub",
                    TimeStamp = activity.CreatedAt,
                    HexSidelineColor = "#000000",
                    Text = attachmentText
                };
                attachments.Add(attachment);
            }

            if (attachments.Count == 0)
            {
                message = null;
                return false;
            }

            message = new SlackMessage
            {
                UserName = "GitHub",
                IconUrl = _configuration.IconUrl,
                Attachments = attachments
            };

            return true;
        }
    }
}
