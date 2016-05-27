using System;

namespace PostBot.Configuration
{
    public class GithubConfiguration : MonitorConfiguration
    {
        public string Token { get; set; }

        public string Organization { get; set; }

        public Uri IconUrl { get; set; }
    }
}
