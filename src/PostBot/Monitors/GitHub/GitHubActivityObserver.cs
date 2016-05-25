using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Octokit;
using PostBot.Monitors.Internal;

namespace PostBot.Monitors.GitHub
{
    public class GitHubActivityObserver : IObserver<Activity>
    {
        private readonly SafeCancellationTokenSource _cancellationToken;
        private readonly DateTimeOffset _lastObservation;
        private readonly ILogger _logger;
        private readonly List<Activity> _observedActivities;

        public GitHubActivityObserver(
            ILogger logger,
            SafeCancellationTokenSource cancellationToken, 
            DateTimeOffset lastObservation)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
            _lastObservation = lastObservation;
            _observedActivities = new List<Activity>();
        }

        public IReadOnlyList<Activity> ObservedActivities => _observedActivities;

        public void OnCompleted()
        {
            _logger.LogInformation($" -- GitHub monitoring complete. {_observedActivities.Count} activities observed.");
            _cancellationToken.Cancel(useNewThread: false);
        }

        public void OnError(Exception error)
        {
            _logger.LogError(
$@"-- Error Occurred --
{error.ToString()}");
        }

        public void OnNext(Activity value)
        {
            if (value.CreatedAt < _lastObservation)
            {
                OnCompleted();
                return;
            }

            _logger.LogInformation(
$@"-- Observed Activity --
At: {value.CreatedAt.TimeOfDay}
Repo: {value.Repo.Name}
Name: {value.Actor.Login}
From: {value.Type}");

            _observedActivities.Add(value);
        }
    }
}
