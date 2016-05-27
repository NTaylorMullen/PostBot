using System;
using System.Threading;
using PostBot.Configuration;
using PostBot.Slack;

namespace PostBot.Monitors
{
    public abstract class Monitor : IDisposable
    {
        private readonly Timer _timer;
        private int _polling;

        public Monitor(SlackClient client, MonitorConfiguration configuration)
        {
            var pollPeriod = TimeSpan.FromMilliseconds(configuration.PollPeriod);
            _timer = new Timer(Poll, state: null, dueTime: TimeSpan.FromSeconds(1), period: pollPeriod);

            Client = client;
        }

        protected SlackClient Client { get; }

        protected abstract void Poll();

        private void Poll(object state)
        {
            try
            {
                if (Interlocked.Exchange(ref _polling, 1) == 1)
                {
                    return;
                }

                Poll();
            }
            finally
            {
                Interlocked.Exchange(ref _polling, 0);
            }
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
