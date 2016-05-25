using Microsoft.Extensions.Logging.Console;
using Octokit;
using Octokit.Reactive;
using PostBot.Configuration;
using PostBot.DataFilters;

namespace PostBot.Monitors.GitHub
{
    public class GitHubMonitor : Monitor
    {
        private readonly ObservableActivitiesClient _client;
        private readonly GithubConfiguration _configuration;
        private readonly GitHubActivityProvider _eventObserver;
        private readonly GitHubDataFilter _dataFilter;

        public GitHubMonitor(GithubConfiguration configuration)
            : base(configuration)
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
        }

        protected override void Poll()
        {
            var observable = _client.Events.GetAllForOrganization(_configuration.Organization);

            var activities = _eventObserver.GetActivities(observable);
            var filteredActivities = _dataFilter.Filter(activities);

        }
    }
}
