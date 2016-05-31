using System;

namespace PostBot.Configuration
{
    public class ApplicationConfiguration
    {
        public SlackConfiguration Slack { get; set; }

        public GithubConfiguration GitHub { get; set; }

        public TeamCityConfiguration TeamCity { get; set; }

        public StackOverflowConfiguration StackOverflow { get; set; }

        public AspNetForumConfiguration AspNetForums { get; set; }
    }
}
