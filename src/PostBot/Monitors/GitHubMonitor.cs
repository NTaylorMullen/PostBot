using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using PostBot.Configuration;

namespace PostBot.Monitors
{
    public class GitHubMonitor : Monitor
    {
        private readonly GitHubClient _client;

        public GitHubMonitor(GithubConfiguration configuration)
            : base(configuration)
        {
            var credentials = new Credentials(configuration.Token);
            _client = new GitHubClient(new ProductHeaderValue("post-bot"))
            {
                Credentials = credentials
            };
        }

        protected override void Poll()
        {
            
        }
    }
}
