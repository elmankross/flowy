using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Tests.Activities
{
    public sealed class Accepts<TValue> : IActivity<TValue>
    {
        public TValue Value { get; private set; }

        public Task ExecuteAsync(TValue accept, Func<Task> next, CancellationToken token)
        {
            Value = accept;
            return next();
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => ExecuteAsync(default, next, token);
    }
}
