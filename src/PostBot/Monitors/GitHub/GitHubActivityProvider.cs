using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Octokit;
using PostBot.Monitors.Internal;

namespace PostBot.Monitors.GitHub
{
    public class GitHubActivityProvider
    {
        private DateTimeOffset _lastObservation;
        private readonly ILogger _logger;

        public GitHubActivityProvider(ILogger logger)
        {
            _lastObservation = DateTime.UtcNow;
            _logger = logger;
        }

        public IReadOnlyList<Activity> GetActivities(IObservable<Activity> observable)
        {
            var cancellationToken = new SafeCancellationTokenSource();
            var activityObserver = new GitHubActivityObserver(_logger, cancellationToken, _lastObservation);

            observable.Subscribe(activityObserver, cancellationToken.Token);

            cancellationToken.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            var observedActivities = activityObserver.ObservedActivities;
            if (observedActivities.Count > 0)
            {
                foreach (var observedActivity in observedActivities)
                {
                    if (observedActivity.CreatedAt > _lastObservation)
                    {
                        _lastObservation = observedActivity.CreatedAt;
                    }
                }

                _lastObservation = _lastObservation.AddMilliseconds(1);
            }

            return activityObserver.ObservedActivities;
        }
    }
}
