using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class Converter<TSource, TResult> : IActivity<TSource, TResult>
    {
        public Func<TSource, TResult> Selector { get; }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Converter");
            return next();
        }

        public Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Converter{T}");
            return ExecuteAsync(source, _ => next(), token);
        }

        public Task ExecuteAsync(TSource source, Func<TResult, Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "Converter{T1,T2}");
            return next(Selector(source));
        }
    }
}
