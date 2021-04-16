using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class Variable<TValue> : IActivity<TValue, TValue>
    {
        public TValue Value { get; set; }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Variable");
            return next();
        }

        public Task ExecuteAsync(TValue source, Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Variable{T}");
            return ExecuteAsync(source, _ => next(), token);
        }

        public Task ExecuteAsync(TValue source, Func<TValue, Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Variable{T1,T2}");
            Value = source;
            return next(Value);
        }
    }
}
