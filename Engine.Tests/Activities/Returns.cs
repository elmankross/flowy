using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Tests.Activities
{
    public sealed class Returns<TAccept, TReturn> : IActivity<TAccept, TReturn>
    {
        public Func<TAccept, TReturn> Selector { private get; set; }

        public Task ExecuteAsync(TAccept accept, Func<TReturn, Task> next, CancellationToken token)
        {
            return next(Selector(accept));
        }

        public Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token)
            => ExecuteAsync(accept, _ => next(), token);

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => ExecuteAsync(default, next, token);
    }
}
