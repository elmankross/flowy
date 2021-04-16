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
    /// Activity builder for activities uses source and generates result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IActivityBuilder<TActivity, TSource, TResult> : IActivityBuilder<TActivity, TSource>
        where TActivity : IActivity<TSource, TResult>
    {
        new IActivity<TSource, TResult> Build();
    }


    /// <summary>
    /// Activity builder for activities uses source and generates no result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface IActivityBuilder<TActivity, TSource> : IActivityBuilder
        where TActivity : IActivity<TSource>
    {
        new IActivity<TSource> Build();

        /// <summary>
        /// Use new activity with new result
        /// </summary>
        /// <typeparam name="TNextActivity"></typeparam>
        /// <typeparam name="TNewResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TNextActivity, TSource, TNewResult> Then<TNextActivity, TNewResult>(
            Action<TNextActivity> builder,
            Func<TSource, TNewResult> selector)
            where TNextActivity : IActivity<TSource, TNewResult>, new();
    }


    /// <summary>
    /// Activity builder for activities uses no source and generates no result
    /// </summary>
    public interface IActivityBuilder
    {
        IActivity Build();

        /// <summary>
        /// Use activity without source and result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder Then<TActivity>(
            Action<TActivity> builder)
            where TActivity : IActivity, new();

        /// <summary>
        /// Use activity with source and no result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource> Then<TActivity, TSource>(
            Action<TActivity> builder)
            where TActivity : IActivity<TSource>, new();

        /// <summary>
        /// Use activity with soure and result
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
    /// Activity builder for activities uses source and generates result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class ActivityBuilder<TActivity, TSource, TResult> : ActivityBuilder<TActivity, TSource>
        , IActivityBuilder<TActivity, TSource, TResult>
        where TActivity : IActivity<TSource, TResult>
    {
        new protected readonly IActivity _current;
        new protected readonly IActivity<TSource, TResult> _next;

        IActivity<TSource, TResult> IActivityBuilder<TActivity, TSource, TResult>.Build()
            => new Activity<TSource, TResult>(_current, _next);

        public ActivityBuilder(IActivity current, IActivity<TSource, TResult> next)
            : base(current, null)
        {
            _current = current;
            _next = next;
        }
    }


    /// <summary>
    /// Activity builder for activities uses source and generates no result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public class ActivityBuilder<TActivity, TSource> : ActivityBuilder
        , IActivityBuilder<TActivity, TSource>
        where TActivity : IActivity<TSource>
    {
        new protected readonly IActivity<TSource> _current;
        protected readonly IActivity _next;

        IActivity<TSource> IActivityBuilder<TActivity, TSource>.Build()
            => new Activity<TSource>(_current, _next);

        public ActivityBuilder(IActivity current, IActivity next)
            : base(current)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity next)
            : base(null)
        {
            _current = current;
            _next = next;
        }

        public IActivityBuilder<TNextActivity, TSource, TNewResult> Then<TNextActivity, TNewResult>(
            Action<TNextActivity> builder,
            Func<TSource, TNewResult> selector)
            where TNextActivity : IActivity<TSource, TNewResult>, new()
        {
            var current = new Activity<TSource>(_current ?? base._current, _next);
            var next = CreateActivity(builder);
            return new ActivityBuilder<TNextActivity, TSource, TNewResult>(current, next);
        }
    }


    /// <summary>
    /// Activity builder for activities uses no source and generates no result
    /// </summary>
    public class ActivityBuilder : IActivityBuilder
    {
        protected readonly IActivity _current;

        public ActivityBuilder(IActivity current)
        {
            _current = current;
        }

        public IActivity Build() => new Activity(_current, null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IActivityBuilder Then<TActivity>(Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var next = CreateActivity(builder);
            return new ActivityBuilder(next);
        }

        /// <summary>
        /// Use activity without result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IActivityBuilder<TActivity, TSource> Then<TActivity, TSource>(Action<TActivity> builder)
            where TActivity : IActivity<TSource>, new()
        {
            var next = CreateActivity(builder);
            return new ActivityBuilder<TActivity, TSource>(_current, next);
        }

        /// <summary>
        /// Use activity with result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IActivityBuilder<TActivity, TSource, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder,
            Func<TSource, TResult> selector)
            where TActivity : IActivity<TSource, TResult>, new()
        {
            var next = CreateActivity(builder);
            return new ActivityBuilder<TActivity, TSource, TResult>(_current, next);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected static TActivity CreateActivity<TActivity>(Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var instance = new TActivity();
            builder(instance);
            return instance;
        }
    }
}
