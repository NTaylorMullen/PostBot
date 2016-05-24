using System;
using System.Collections.Generic;
using Octokit;
using PostBot.Monitors.Internal;

namespace PostBot.Monitors.GitHub
{
    public class GitHubEventObserver
    {
        private DateTimeOffset _lastObservation;

        public GitHubEventObserver()
        {
            _lastObservation = DateTime.UtcNow;
            _lastObservation = _lastObservation.Subtract(TimeSpan.FromHours(4));
        }

        public IReadOnlyList<Activity> GetEvents(IObservable<Activity> observable)
        {
            var observedEvents = new List<Activity>();
            var cancellationToken = new SafeCancellationTokenSource();
            Action onCompleted = () =>
            {
                Console.WriteLine($" -- GitHub monitoring complete. {observedEvents.Count} activities observed.");
                Console.WriteLine();
                cancellationToken.Cancel(useNewThread: false);
            };
            Action<Activity> onNext = activity =>
            {
                if (activity.CreatedAt < _lastObservation)
                {
                    onCompleted();
                    return;
                }

                Console.WriteLine("-- Observed Activity --");
                Console.WriteLine($"\tAt: {activity.CreatedAt.TimeOfDay}");
                Console.WriteLine($"\tRepo: {activity.Repo.Name}");
                Console.WriteLine($"\tName: {activity.Actor.Login}");
                Console.WriteLine($"\tFrom: {activity.Type}");
                Console.WriteLine();

                observedEvents.Add(activity);
            };
            Action<Exception> onError = exception =>
            {
                Console.WriteLine("-- Error Occurred --");
                Console.WriteLine(exception.ToString());
                Console.WriteLine();
            };

            observable.Subscribe(onNext, onError, onCompleted, cancellationToken.Token);

            cancellationToken.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            _lastObservation = DateTime.UtcNow;

            return observedEvents;
        }
    }
}
