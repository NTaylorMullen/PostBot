using System;
using System.Threading;
using PostBot.Configuration;

namespace PostBot.Monitors
{
    public abstract class Monitor
    {
        private readonly Timer _timer;

        public Monitor(MonitorConfiguration configuration)
        {
            var pollPeriod = TimeSpan.FromMilliseconds(configuration.PollPeriod);
            _timer = new Timer(Poll, state: null, dueTime: TimeSpan.Zero, period: pollPeriod);
        }

        protected abstract void Poll();

        private void Poll(object state)
        {
            Poll();
        }
    }
}
