using System;

namespace PostBot.Configuration
{
    public class ApplicationConfiguration
    {
        private string _postChannel;

        public string PostChannel
        {
            get
            {
                return _postChannel;
            }
            set
            {
                if (value[0] != '#')
                {
                    _postChannel = $"#{value}";
                }
                else
                {
                    _postChannel = value;
                }
            }
        }

        public Uri WebHookUrl { get; set; }

        public GithubConfiguration GitHub { get; set; }

        public TeamCityConfiguration TeamCity { get; set; }

        public StackOverflowConfiguration StackOverflow { get; set; }

        public AspNetForumConfiguration AspNetForums { get; set; }
    }
}
