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
        public Task ExecuteAsync(TSource source, Func<TResult, Task> next = default, CancellationToken token = default);
    }

    /// <summary>
    /// Executes with input but has no output
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IActivity<TSource> : IActivity
    {
        public Task ExecuteAsync(TSource source, Func<Task> next = default, CancellationToken token = default);
    }

    /// <summary>
    /// Executes without inputs and has no output
    /// </summary>
    public interface IActivity
    {
        public Task ExecuteAsync(Func<Task> next = default, CancellationToken token = default);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TSourceResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    [DebuggerDisplay("{Current} -> {Next}")]
    public class Activity<TSource, TSourceResult, TNextResult> : Activity<TSource>, IActivity<TSource, TNextResult>
    {
        new public IActivity<TSource, TSourceResult> Current { get; }
        new public IActivity<TSourceResult, TNextResult> Next { get; }

        #region .ctr

        public Activity(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult, TNextResult> next)
            : base(current, next)
        {
            Current = current;
            Next = next;
        }

        public Activity(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult> next)
            : base(current, next)
        {
            Current = current;
        }

        public Activity(IActivity<TSource, TSourceResult> current, IActivity next)
            : base(current, next)
        {
            Current = current;
        }

        public Activity(IActivity<TSource> current, IActivity<TSourceResult, TNextResult> next)
            : base(current, next)
        {
            Next = next;
        }

        public Activity(IActivity current, IActivity<TSourceResult, TNextResult> next)
            : base(current, next)
        {
            Next = next;
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

        public Task ExecuteAsync(TSource source, Func<TNextResult, Task> next = default, CancellationToken token = default)
        {
            next ??= ActivityExtensions.CompleteTask;
            var result = Task.CompletedTask;
            ActivityExtensions.Fork(Current, Next,
                bothExists: () => result = Current.ExecuteAsync(source, result => Next.ExecuteAsync(result, next, token), token),
                currentExists: () => result = Current.ExecuteAsync(source, _ => ExecuteAsync(default, () => next(default), token), token),
                nextExists: () => result = ExecuteAsync(source, () => Next.ExecuteAsync(default, () => next(default), token), token),
                bothEmpty: () => result = ExecuteAsync(source, () => next(default), token));
            return result;
        }
    }


    /// <summary>
    /// Executes with input but has no output
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    [DebuggerDisplay("{Current} -> {Next}")]
    public class Activity<TSource> : Activity, IActivity<TSource>
    {
        new public IActivity<TSource> Current { get; }
        new public IActivity<TSource> Next { get; }

        #region .ctr

        public Activity(IActivity<TSource> current, IActivity<TSource> next)
            : base(current, next)
        {
            Current = current;
            Next = next;
        }

        public Activity(IActivity<TSource> current, IActivity next)
            : base(null, next)
        {
            Current = current;
        }

        public Activity(IActivity current, IActivity<TSource> next)
            : base(current, null)
        {
            Next = next;
        }

        public Activity(IActivity current, IActivity next)
            : base(current, next)
        {
        }

        #endregion

        public Task ExecuteAsync(TSource source, Func<Task> next = default, CancellationToken token = default)
        {
            next ??= ActivityExtensions.EmptyCompleteTask;
            var result = Task.CompletedTask;
            ActivityExtensions.Fork(Current, Next,
                bothExists: () => result = Current.ExecuteAsync(source, () => Next.ExecuteAsync(default, next, token), token),
                currentExists: () => result = Current.ExecuteAsync(source, () => ExecuteAsync(next, token), token),
                nextExists: () => result = ExecuteAsync(() => Next.ExecuteAsync(default, next, token), token),
                bothEmpty: () => result = ExecuteAsync(next, token));
            return result;
        }
    }


    /// <summary>
    /// Executes without inputs and has no output
    /// </summary>
    [DebuggerDisplay("{Current} -> {Next}")]
    public class Activity : IActivity
    {
        public IActivity Current { get; }
        public IActivity Next { get; }

        public Activity(IActivity current, IActivity next)
        {
            Current = current;
            Next = next;
        }

        public virtual Activity<TSource, TSourceResult, TNextResult> ConvertTo<TSource, TSourceResult, TNextResult>()
            => new Activity<TSource, TSourceResult, TNextResult>(Current, Next);

        public virtual Activity<TSource> ConvertTo<TSource>()
            => new Activity<TSource>(Current, Next);

        public Task ExecuteAsync(Func<Task> next = default, CancellationToken token = default)
        {
            next ??= ActivityExtensions.EmptyCompleteTask;
            var result = Task.CompletedTask;
            ActivityExtensions.Fork(Current, Next,
                bothExists: () => result = Current.ExecuteAsync(() => Next.ExecuteAsync(next, token), token),
                currentExists: () => result = Current.ExecuteAsync(next, token),
                nextExists: () => result = Next.ExecuteAsync(next, token),
                bothEmpty: () => result = next());
            return result;
        }
    }
}
