using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public sealed class WriteTo<TAccept> : IActivity<TAccept>, IActivity<TAccept, TAccept>
    {
        public Func<TAccept, Task> Action { private get; set; }

        public async Task ExecuteAsync(TAccept accept, Func<TAccept, Task> next, CancellationToken token)
        {
            await Action(accept);
            await next(accept);
        }

        public Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token)
            => ExecuteAsync(accept, _ => next(), token);

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => ExecuteAsync(default, token);
    }
}