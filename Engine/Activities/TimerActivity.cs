using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    [DebuggerDisplay("Timer")]
    [Metadata(Category = "Triggers", Description = "Executes every times", Name = "Timer")]
    public sealed class TimerActivity : IActivity
    {
        public TimeSpan Interval { get; set; }

        private System.Timers.Timer _timer;

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Timer");
            _timer = new System.Timers.Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += (_, __) => next();
            _timer.Start();
            return next();
        }
    }
}
