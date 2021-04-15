using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    [Metadata(Category = "Triggers", Description = "Executes every times", Name = "Timer")]
    public sealed class Timer : IActivity
    {
        public TimeSpan Interval { get; set; }

        private readonly System.Timers.Timer _timer;

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            _timer.Elapsed += (_, __) => next();
            _timer.Start();
            return Task.CompletedTask;
        }
    }
}
