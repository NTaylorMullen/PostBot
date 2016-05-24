using System;
using System.Threading;

namespace PostBot.Monitors.Internal
{
    public class SafeCancellationTokenSource : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private int _state;

        public SafeCancellationTokenSource()
        {
            _cts = new CancellationTokenSource();
            Token = _cts.Token;
        }

        public CancellationToken Token { get; }

        public void Cancel(bool useNewThread = true)
        {
            var value = Interlocked.CompareExchange(ref _state, State.Cancelling, State.Initial);

            if (value == State.Initial)
            {

                if (!useNewThread)
                {
                    CancelCore();
                    return;
                }

                // Because cancellation tokens are so poorly behaved, always invoke the cancellation token on 
                // another thread. Don't capture any of the context (execution context or sync context)
                // while doing this.
                ThreadPool.UnsafeQueueUserWorkItem(_ =>
                {
                    CancelCore();
                }, state: null);
            }
        }

        private void CancelCore()
        {
            try
            {
                _cts.Cancel();
            }
            finally
            {
                if (Interlocked.CompareExchange(ref _state, State.Cancelled, State.Cancelling) == State.Disposing)
                {
                    _cts.Dispose();
                    Interlocked.Exchange(ref _state, State.Disposed);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var value = Interlocked.Exchange(ref _state, State.Disposing);

                switch (value)
                {
                    case State.Initial:
                    case State.Cancelled:
                        _cts.Dispose();
                        Interlocked.Exchange(ref _state, State.Disposed);
                        break;
                    case State.Cancelling:
                    case State.Disposing:
                        // No-op
                        break;
                    case State.Disposed:
                        Interlocked.Exchange(ref _state, State.Disposed);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private static class State
        {
            public const int Initial = 0;
            public const int Cancelling = 1;
            public const int Cancelled = 2;
            public const int Disposing = 3;
            public const int Disposed = 4;
        }
    }
}
