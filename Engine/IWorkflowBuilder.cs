using System;

namespace Engine
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWorkflowBuilder
    {
        IActivityBuilder When<TSource>(Action<TSource> builder = null)
            where TSource : IActivity, new();
    }


    /// <summary>
    /// Activity builder for activities uses source and generates result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TCurrentResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    public interface IActivityBuilder<TActivity, TSource, TCurrentResult, TNextResult> : IActivityBuilder<TActivity, TSource>
        where TActivity : IActivity<TSource, TNextResult>
    {
        new IActivity<TSource, TNextResult> Build();
    }


    /// <summary>
    /// Activity builder for activities uses source and generates no result
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface IActivityBuilder<TActivity, TSource> : IActivityBuilder
        where TActivity : IActivity<TSource>
    {
        new Activity<TSource> Build();
    }


    /// <summary>
    /// Activity builder for activities uses no source and generates no result
    /// </summary>
    public interface IActivityBuilder
    {
        Activity Build();

        /// <summary>
        /// Use activity without source and result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder Then<TActivity>(
            Action<TActivity> builder = null)
            where TActivity : IActivity, new();

        /// <summary>
        /// Use activity with source and no result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource> Then<TActivity, TSource>(
            Action<TActivity> builder = null)
            where TActivity : IActivity<TSource>, new();

        /// <summary>
        /// Use activity with soure and same result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource, TSource, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder = null)
            where TActivity : IActivity<TSource, TResult>, new();
    }


    /// <summary>
    /// 
    /// </summary>
    public class WorkflowBuilder : IWorkflowBuilder
    {
        public IActivityBuilder When<TActivity>(Action<TActivity> builder = null)
            where TActivity : IActivity, new()
        {
            return new ActivityBuilder(null, builder.CreateActivity());
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TCurrentResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    public class ActivityBuilder<TActivity, TSource, TCurrentResult, TNextResult> : ActivityBuilder<TActivity, TSource>
        , IActivityBuilder<TActivity, TSource, TCurrentResult, TNextResult>
        where TActivity : IActivity<TSource, TNextResult>
    {
        private readonly IActivity<TSource, TCurrentResult> _current;
        private readonly IActivity<TCurrentResult, TNextResult> _next;

        #region .ctr from Activity{T1,T2,T3}

        public ActivityBuilder(IActivity<TSource, TCurrentResult> current, IActivity<TCurrentResult, TNextResult> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource, TCurrentResult> current, IActivity<TCurrentResult> next)
            : base(null, next)
        {
            _current = current;
        }

        public ActivityBuilder(IActivity<TSource, TCurrentResult> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity<TCurrentResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity current, IActivity<TCurrentResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity<TCurrentResult> next)
            : base(current, next)
        {
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity next)
            : base(current, next)
        {
        }

        //public ActivityBuilder(IActivity current, IActivity<TCurrentResult> next)
        //    : base(current, next)
        //{
        //}

        //public ActivityBuilder(IActivity current, IActivity next)
        //    : base(current, next)
        //{
        //}

        #endregion

        IActivity<TSource, TNextResult> IActivityBuilder<TActivity, TSource, TCurrentResult, TNextResult>.Build()
            => GetBoxedCurrentActivity();

        public override IActivityBuilder Then<TNewActivity>(Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder(current, next);
        }

        override public IActivityBuilder<TNewActivity, TNewSource> Then<TNewActivity, TNewSource>(
            Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TNewActivity, TNewSource>(current, next);
        }

        public override IActivityBuilder<TNewActivity, TNewSource, TNewSource, TNewResult> Then<TNewActivity, TNewSource, TNewResult>(Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TNewActivity, TNewSource, TNewSource, TNewResult>(current, next);
        }

        private Activity<TSource, TCurrentResult, TNextResult> GetBoxedCurrentActivity()
        {
            Activity<TSource, TCurrentResult, TNextResult> current = null;
            ActivityExtensions.Fork(_current, _next,
                bothExists: () => current = new Activity<TSource, TCurrentResult, TNextResult>(_current, _next),
                currentExists: () =>
                {
                    var second = SecondLayer.Build();
                    if (second.Next != null)
                    {
                        current = new Activity<TSource, TCurrentResult, TNextResult>(_current, second.Next);
                    }
                    else
                    {
                        current = new Activity<TSource, TCurrentResult, TNextResult>(_current, ((Activity)second).Next);
                    }
                },
                nextExists: () =>
                {
                    var second = SecondLayer.Build();
                    if (second.Current != null)
                    {
                        current = new Activity<TSource, TCurrentResult, TNextResult>(second.Current, _next);
                    }
                    else
                    {
                        current = new Activity<TSource, TCurrentResult, TNextResult>(((Activity)second).Current, _next);
                    }
                },
                bothEmpty: () => current = SecondLayer.Build().ConvertTo<TSource, TCurrentResult, TNextResult>());
            return current;
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
        protected IActivityBuilder<TActivity, TSource> SecondLayer { get; }
        private readonly IActivity<TSource> _current;
        private readonly IActivity<TSource> _next;

        #region .ctr from Activity{T}

        public ActivityBuilder(IActivity<TSource> current, IActivity<TSource> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
            SecondLayer = this;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity next)
            : base(null, next)
        {
            _current = current;
            SecondLayer = this;
        }

        public ActivityBuilder(IActivity current, IActivity<TSource> next)
            : base(current, null)
        {
            _next = next;
            SecondLayer = this;
        }


        //public ActivityBuilder(IActivity current, IActivity next)
        //    : base(current, next)
        //{
        //}

        #endregion

        Activity<TSource> IActivityBuilder<TActivity, TSource>.Build()
            => GetBoxedCurrentActivity();

        public override IActivityBuilder Then<TNewActivity>(Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder(current, next);
        }

        public override IActivityBuilder<TNewActivity, TNewSource> Then<TNewActivity, TNewSource>(
            Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TNewActivity, TNewSource>(current, next);
        }

        public override IActivityBuilder<TNewActivity, TNewSource, TNewSource, TNewResult> Then<TNewActivity, TNewSource, TNewResult>(
            Action<TNewActivity> builder = null)
        {
            var current = GetBoxedCurrentActivity();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TNewActivity, TNewSource, TNewSource, TNewResult>(current, next);
        }

        private Activity<TSource> GetBoxedCurrentActivity()
        {
            Activity<TSource> current = null;
            ActivityExtensions.Fork(_current, _next,
                bothExists: () => current = new Activity<TSource>(_current, _next),
                currentExists: () => current = new Activity<TSource>(_current, FirstLayer.Build().Next),
                nextExists: () => current = new Activity<TSource>(FirstLayer.Build().Current, _next),
                bothEmpty: () => current = FirstLayer.Build().ConvertTo<TSource>());
            return current;
        }
    }


    /// <summary>
    /// Activity builder for activities uses no source and generates no result
    /// </summary>
    public class ActivityBuilder : IActivityBuilder
    {
        protected IActivityBuilder FirstLayer { get; }
        private readonly IActivity _current;
        private readonly IActivity _next;

        public ActivityBuilder(IActivity current, IActivity next)
        {
            _current = current;
            _next = next;
            FirstLayer = this;
        }

        public Activity Build() => new Activity(_current, _next);

        public virtual IActivityBuilder Then<TActivity>(
            Action<TActivity> builder = null)
            where TActivity : IActivity, new()
        {
            var current = Build();
            var next = builder.CreateActivity();
            return new ActivityBuilder(current, next);
        }

        public virtual IActivityBuilder<TActivity, TSource> Then<TActivity, TSource>(
            Action<TActivity> builder = null)
            where TActivity : IActivity<TSource>, new()
        {
            var current = Build();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TActivity, TSource>(current, next);
        }

        public virtual IActivityBuilder<TActivity, TSource, TSource, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder = null)
            where TActivity : IActivity<TSource, TResult>, new()
        {
            var current = Build();
            var next = builder.CreateActivity();
            return new ActivityBuilder<TActivity, TSource, TSource, TResult>(current, next);
        }
    }
}
