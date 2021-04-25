using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public sealed class ConvertTo<TAccept, TReturn> : IActivity<TAccept, TReturn>
    {
        public IActivityMeta Meta { get; } = new ActivityMeta
        {
            [ActivityMeta.Key.Name] = "Converter",
            [ActivityMeta.Key.Description] = "Converts one value type to another one"
        };

        public Func<TAccept, Task<TReturn>> Converter { private get; set; }

        public async Task ExecuteAsync(TAccept accept, Func<TReturn, Task> next, CancellationToken token)
        {
            var converted = await Converter(accept);
            await next(converted);
        }

        public Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token)
            => ExecuteAsync(accept, _ => next(), token);

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => ExecuteAsync(default, next, token);
    }
}
