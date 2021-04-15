using System;

namespace Engine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWorkflowBuilder
    {
        IActivityBuilder When<TSource>(Action<TSource> builder)
            where TSource : IActivity, new();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IActivityBuilder<TActivity, TSource, TResult>
        where TActivity : IActivity<TSource, TResult>
    {
        /// <summary>
        /// Use new target
        /// </summary>
        /// <typeparam name="TNextActivity"></typeparam>
        /// <typeparam name="TNextResult"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TNextActivity, TSource, TNextResult> Then<TNextActivity, TNextResult>(
            Action<TNextActivity> builder)
            where TNextActivity : IActivity<TSource, TNextResult>, new();

        /// <summary>
        /// Use new target
        /// </summary>
        /// <typeparam name="TNextActivity"></typeparam>
        /// <typeparam name="TNextResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TNextActivity, TNextResult, TNextResult> Then<TNextActivity, TNextResult>(
            Action<TNextActivity> builder,
            Func<TSource, TNextResult> selector)
            where TNextActivity : IActivity<TNextResult, TNextResult>, new();
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IActivityBuilder
    {
        /// <summary>
        /// Use the same target's source
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource, TSource> Then<TActivity, TSource>(
            Action<TActivity> builder)
            where TActivity : IActivity<TSource, TSource>, new();

        /// <summary>
        /// Use target with new type of target
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder,
            Func<TSource, TResult> selector)
            where TActivity : IActivity<TSource, TResult>, new();
    }


    /// <summary>
    /// 
    /// </summary>
    public class WorkflowBuilder : IWorkflowBuilder
    {
        public IActivityBuilder When<TSource>(Action<TSource> builder)
            where TSource : IActivity, new()
        {
            var instance = new TSource();
            builder(instance);
            return new ActivityBuilder(instance);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ActivityBuilder<TActivity, TSource, TResult>
        : IActivityBuilder<TActivity, TSource, TResult>
        where TActivity : IActivity<TSource, TResult>
    {
        private readonly IActivity _current;
        private readonly IActivity<TSource, TResult> _next;

        public ActivityBuilder(IActivity current, IActivity<TSource, TResult> next)
        {
            _current = current;
            _next = next;
        }

        public IActivityBuilder<TNextActivity, TSource, TNextResult> Then<TNextActivity, TNextResult>(Action<TNextActivity> builder)
            where TNextActivity : IActivity<TSource, TNextResult>, new()
        {
            var next = new TNextActivity();
            builder(next);
            var current = new Activity(_current, _next);
            return new ActivityBuilder<TNextActivity, TSource, TNextResult>(current, next);
        }

        public IActivityBuilder<TNextActivity, TNextResult, TNextResult> Then<TNextActivity, TNextResult>(
            Action<TNextActivity> builder,
            Func<TSource, TNextResult> selector)
            where TNextActivity : IActivity<TNextResult, TNextResult>, new()
        {
            var next = new TNextActivity();
            builder(next);
            var current = new Activity(_current, _next);
            return new ActivityBuilder<TNextActivity, TNextResult, TNextResult>(current, next);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ActivityBuilder : IActivityBuilder
    {
        private readonly IActivity _current;

        public ActivityBuilder(IActivity current)
        {
            _current = current;
        }

        public IActivityBuilder<TActivity, TSource, TSource> Then<TActivity, TSource>(Action<TActivity> builder)
            where TActivity : IActivity<TSource, TSource>, new()
        {
            var next = new TActivity();
            builder(next);
            return new ActivityBuilder<TActivity, TSource, TSource>(_current, next);
        }

        public IActivityBuilder<TActivity, TSource, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder,
            Func<TSource, TResult> selector)
            where TActivity : IActivity<TSource, TResult>, new()
        {
            var next = new TActivity();
            builder(next);
            return new ActivityBuilder<TActivity, TSource, TResult>(_current, next);
        }
    }
}
