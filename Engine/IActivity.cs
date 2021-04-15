using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    public interface IActivity<TSource, TResult> : IActivity
    {
        public abstract Task ExecuteAsync(TSource source, Func<TResult, Task> next, CancellationToken token = default);
    }

    public interface IActivity
    {
        public abstract Task ExecuteAsync(Func<Task> next, CancellationToken token = default);
    }

    public class Activity : IActivity
    {
        private readonly IActivity _current;
        private readonly IActivity _next;

        public Activity(IActivity current, IActivity next)
        {
            _current = current;
            _next = next;
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
