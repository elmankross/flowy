using System;
using System.Diagnostics;
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
    /// Executes with input but has no output
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
    /// <typeparam name="TSourceResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    public class Activity<TSource, TSourceResult, TNextResult> : Activity<TSource>, IActivity<TSource, TNextResult>
    {
        private readonly IActivity<TSource, TSourceResult> _current;
        private readonly IActivity<TSourceResult, TNextResult> _next;

        #region .ctr

        public Activity(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult, TNextResult> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public Activity(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult> next)
            : base(null, next)
        {
            _current = current;
        }

        public Activity(IActivity<TSource, TSourceResult> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public Activity(IActivity<TSource> current, IActivity<TSourceResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public Activity(IActivity current, IActivity<TSourceResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public Activity(IActivity<TSource> current, IActivity<TSourceResult> next)
            : base(current, next)
        {
        }

        public Activity(IActivity<TSource> current, IActivity next)
            : base(current, next)
        {
        }

        public Activity(IActivity current, IActivity<TSourceResult> next)
            : base(current, next)
        {
        }

        public Activity(IActivity current, IActivity next)
            : base(current, next)
        {
        }

        #endregion

        public Task ExecuteAsync(TSource source, Func<TNextResult, Task> next, CancellationToken token = default)
        {
            if (_current != null && _next != null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is presented", "Activity{T1,T2,T3}");
                return _current.ExecuteAsync(source, result => _next.ExecuteAsync(result, next, token), token);
            }
            else if (_current != null && _next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", "Activity{T1,T2,T3}");
                return _current.ExecuteAsync(source, _ => ExecuteAsync(default, () => next(default), token), token);
            }
            else if (_current == null && _next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", "Activity{T1,T2,T3}");
                return ExecuteAsync(source, () => _next.ExecuteAsync(default, () => next(default), token), token);
            }
            else if (_current == null && _next == null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", "Activity{T1,T2,T3}");
                return ExecuteAsync(source, () => next(default), token);
            }
            Debug.WriteLine("Invalid event!", "Activity{T1,T2,T3}");
            return Task.CompletedTask;
        }
    }


    /// <summary>
    /// Executes with input but has no output
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class Activity<TSource> : Activity, IActivity<TSource>
    {
        private readonly IActivity<TSource> _current;
        private readonly IActivity<TSource> _next;

        #region .ctr

        public Activity(IActivity<TSource> current, IActivity<TSource> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public Activity(IActivity<TSource> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public Activity(IActivity current, IActivity<TSource> next)
            : base(current, null)
        {
            _next = next;
        }

        public Activity(IActivity current, IActivity next)
            : base(current, next)
        {
        }

        #endregion

        public Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default)
        {
            if (_current != null && _next != null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is presented", "Activity{T}");
                return _current.ExecuteAsync(source, () => _next.ExecuteAsync(default, next, token), token);
            }
            else if (_current != null && _next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", "Activity{T}");
                return _current.ExecuteAsync(source, () => ExecuteAsync(next, token), token);
            }
            else if (_current == null && _next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", "Activity{T}");
                return ExecuteAsync(() => _next.ExecuteAsync(default, next, token), token);
            }
            else if (_current == null && _next == null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", "Activity{T}");
                return ExecuteAsync(next, token);
            }
            Debug.WriteLine("Invalid event!", "Activity{T}");
            return Task.CompletedTask;
        }
    }


    /// <summary>
    /// Executes without inputs and has no output
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
                Debug.WriteLine("[Current] is presented AND [Next] is presented", "Activity");
                return _current.ExecuteAsync(() => _next.ExecuteAsync(next, token), token);
            }
            else if (_current != null && _next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", "Activity");
                return _current.ExecuteAsync(next, token);
            }
            else if (_current == null && _next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", "Activity");
                return _next.ExecuteAsync(next, token);
            }
            else if (_current == null && _next == null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", "Activity");
                return next();
            }
            Debug.WriteLine("Invalid event!", "Activity");
            return Task.CompletedTask;
        }
    }
}
