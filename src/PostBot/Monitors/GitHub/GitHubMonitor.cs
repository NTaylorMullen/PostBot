using Microsoft.Extensions.Logging.Console;
using Octokit;
using Octokit.Reactive;
using PostBot.Configuration;
using PostBot.DataFilters;
using PostBot.Posters;

namespace PostBot.Monitors.GitHub
{
    public class GitHubMonitor : Monitor
    {
        private readonly ObservableActivitiesClient _client;
        private readonly GithubConfiguration _configuration;
        private readonly GitHubActivityProvider _eventObserver;
        private readonly GitHubDataFilter _dataFilter;
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
            _eventObserver = new GitHubActivityProvider(logger);
            _dataFilter = new GitHubDataFilter();
            _slackMessageProvider = new GitHubSlackMessageProvider(configuration);
        }

        protected override void Poll()
        {
            var observable = _client.Events.GetAllForOrganization(_configuration.Organization);

            var activities = _eventObserver.GetActivities(observable);
            var filteredActivities = _dataFilter.Filter(activities);
            var slackMessage = _slackMessageProvider.GetMessage(filteredActivities);

            Client.Post(slackMessage);
        }
    }
}
