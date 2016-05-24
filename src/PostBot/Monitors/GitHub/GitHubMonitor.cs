using Octokit;
using Octokit.Reactive;
using PostBot.Configuration;

namespace PostBot.Monitors.GitHub
{
    public class GitHubMonitor : Monitor
    {
        private readonly ObservableActivitiesClient _client;
        private readonly GithubConfiguration _configuration;
        private readonly GitHubEventObserver _eventObserver;

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
            _eventObserver = new GitHubEventObserver();
        }

        protected override void Poll()
        {
            var observable = _client.Events.GetAllForOrganization(_configuration.Organization);

            var events = _eventObserver.GetEvents(observable);
        }
    }
}
