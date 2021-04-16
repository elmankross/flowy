using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Executes with input and has output
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IActivity<TSource, TResult> : IActivity<TSource>
    {
        public Task ExecuteAsync(TSource source, Func<TResult, Task> next, CancellationToken token = default);
    }

    /// <summary>
    /// Executes with input bu has no output
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IActivity<TSource> : IActivity
    {
        public Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default);
    }

    /// <summary>
    /// Executes without inputs and has no output
    /// </summary>
    public interface IActivity
    {
        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class Activity<TSource, TResult> : Activity, IActivity<TSource, TResult>
    {
        private readonly IActivity<TSource, TResult> _current;
        private readonly IActivity<TSource, TResult> _next;

        public Activity(IActivity<TSource, TResult> current, IActivity<TSource, TResult> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public Activity(IActivity current, IActivity<TSource, TResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public Activity(IActivity<TSource, TResult> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(TSource source, Func<TResult, Task> next, CancellationToken token = default)
        {
            if (_current != null && _next != null)
            {
                System.Diagnostics.Debug.WriteLine("_current && _next == null", "Activity{,}");
                return _current.ExecuteAsync(source, result => _next.ExecuteAsync(default, next, token), token);
            }
            else if (_current != null && _next == null)
            {
                System.Diagnostics.Debug.WriteLine("_current != null && _next == null", "Activity{,}");
                return _current.ExecuteAsync(source, result => base.ExecuteAsync(() => next(result), token), token);
            }
            else if (_current == null && _next != null)
            {
                System.Diagnostics.Debug.WriteLine("_current == null && _next != null", "Activity{,}");
                return base.ExecuteAsync(() => _next.ExecuteAsync(() => next(default), token), token);
            }
            return Task.CompletedTask;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class Activity<TSource> : Activity, IActivity<TSource>
    {
        public Activity(IActivity<TSource> source, IActivity next)
            :base(null, next)
        {
        }

        public Activity(IActivity source, IActivity next)
            :base(source, next)
        {
        }

        public Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 
    /// </summary>
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
            if (_current != null && _next != null)
            {
                System.Diagnostics.Debug.WriteLine("_current && _next == null", "Activity");
                return _current.ExecuteAsync(() => _next.ExecuteAsync(next, token), token);
            }
            else if (_current != null && _next == null)
            {
                System.Diagnostics.Debug.WriteLine("_current != null && _next == null", "Activity");
                return _current.ExecuteAsync(next, token);
            }
            else if (_current == null && _next != null)
            {
                System.Diagnostics.Debug.WriteLine("_current == null && _next != null", "Activity");
                return _next.ExecuteAsync(next, token);
            }
            return Task.CompletedTask;
        }
    }
}
