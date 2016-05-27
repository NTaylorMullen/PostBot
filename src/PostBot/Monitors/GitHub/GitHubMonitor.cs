using System.Linq;
using Microsoft.Extensions.Logging.Console;
using Octokit;
using Octokit.Reactive;
using PostBot.Configuration;
using PostBot.Slack;

namespace PostBot.Monitors.GitHub
{
    public class GitHubMonitor : Monitor
    {
        private readonly ObservableActivitiesClient _client;
        private readonly GithubConfiguration _configuration;
        private readonly GitHubActivityProvider _activityObserver;
        private readonly GitHubSlackMessageProvider _slackMessageProvider;

        public GitHubMonitor(SlackClient client, GithubConfiguration configuration)
            : base(client, configuration)
        {
            _configuration = configuration;

            var credentials = new Credentials(_configuration.Token);
            var gitHubClient = new GitHubClient(new ProductHeaderValue("post-bot"))
            {
                Credentials = credentials
            };
            _client = new ObservableActivitiesClient(gitHubClient);

            var logger = new ConsoleLogger("GitHub", (name, level) => true, includeScopes: true);
            _activityObserver = new GitHubActivityProvider(logger);
            _slackMessageProvider = new GitHubSlackMessageProvider(logger, configuration);
        }

        protected override void Poll()
        {
            var observable = _client.Events.GetAllForOrganization(_configuration.Organization);

            var activities = _activityObserver.GetActivities(observable);
            if (activities.Any())
            {
                SlackMessage slackMessage;
                if (_slackMessageProvider.TryGetMessage(activities, out slackMessage))
                {
                    Client.Post(slackMessage);
                }
            }
        }
    }
}
