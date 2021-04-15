using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class Variable<TValue> : IActivity<TValue, TValue>
    {
        public TValue Value { get; set; }

        public Task ExecuteAsync(TValue source, Func<TValue, Task> next, CancellationToken token = default)
        {
            Value = source;
            return next(Value);
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            return next();
        }
    }
}
