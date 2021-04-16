using System;
using System.Diagnostics;

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
    /// <typeparam name="TSourceResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    public interface IActivityBuilder<TActivity, TSource, TSourceResult, TNextResult> : IActivityBuilder<TActivity, TSource>
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
        new IActivity<TSource> Build();

        /// <summary>
        /// Use new activity with new result
        /// </summary>
        /// <typeparam name="TNextActivity"></typeparam>
        /// <typeparam name="TNewResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TNextActivity, TSource, TNewResult, TNewResult> Then<TNextActivity, TNewResult>(
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
        /// Use activity with soure and same result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource, TResult, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder,
            Func<TSource, TResult> selector)
            where TActivity : IActivity<TSource, TResult>, new();

        /// <summary>
        /// Use activity with soure and same result
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActivityBuilder<TActivity, TSource, TSourceResult, TNextResult> Then<TActivity, TSource, TSourceResult, TNextResult>(
            Action<TActivity> builder,
            Func<TSource, TNextResult> selector)
            where TActivity : IActivity<TSource, TNextResult>, new();
    }


    /// <summary>
    /// 
    /// </summary>
    public class WorkflowBuilder : IWorkflowBuilder
    {
        public IActivityBuilder When<TActivity>(Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            return new ActivityBuilder(builder.SetupActivity(), null);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TSourceResult"></typeparam>
    /// <typeparam name="TNextResult"></typeparam>
    public class ActivityBuilder<TActivity, TSource, TSourceResult, TNextResult> : ActivityBuilder<TActivity, TSource>
        , IActivityBuilder<TActivity, TSource, TSourceResult, TNextResult>
        where TActivity : IActivity<TSource, TNextResult>
    {
        new protected readonly IActivity<TSource, TSourceResult> _current;
        new protected readonly IActivity<TSourceResult, TNextResult> _next;

        #region .ctr from Activity{T1,T2,T3}

        public ActivityBuilder(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult, TNextResult> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource, TSourceResult> current, IActivity<TSourceResult> next)
            : base(null, next)
        {
            _current = current;
        }

        public ActivityBuilder(IActivity<TSource, TSourceResult> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity<TSourceResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity current, IActivity<TSourceResult, TNextResult> next)
            : base(current, null)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity<TSourceResult> next)
            : base(current, next)
        {
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity next)
            : base(current, next)
        {
        }

        public ActivityBuilder(IActivity current, IActivity<TSourceResult> next)
            : base(current, next)
        {
        }

        public ActivityBuilder(IActivity current, IActivity next)
            : base(current, next)
        {
        }

        #endregion

        IActivity<TSource, TNextResult> IActivityBuilder<TActivity, TSource, TSourceResult, TNextResult>.Build()
        {
            if (_current != null && _next != null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is presented", "ActivityBuilder{T1,T2,T3,T4}");
                return new Activity<TSource, TSourceResult, TNextResult>(_current, _next);
            }
            else if (_current != null && _next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", "ActivityBuilder{T1,T2,T3,T4}");
                return new Activity<TSource, TSourceResult, TNextResult>(_current, base._next);
            }
            else if (_current == null && _next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", "ActivityBuilder{T1,T2,T3,T4}");
                return new Activity<TSource, TSourceResult, TNextResult>(base._current, _next);
            }
            else if (_current == null && _next == null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", "ActivityBuilder{T1,T2,T3,T4}");
                return new Activity<TSource, TSourceResult, TNextResult>(base._current, base._next);
            }

            throw new NotImplementedException("There is no suitable build methods.");
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
        new protected readonly IActivity<TSource> _next;

        #region .ctr from Activity{T}

        public ActivityBuilder(IActivity<TSource> current, IActivity<TSource> next)
            : base(null, null)
        {
            _current = current;
            _next = next;
        }

        public ActivityBuilder(IActivity<TSource> current, IActivity next)
            : base(null, next)
        {
            _current = current;
        }

        public ActivityBuilder(IActivity current, IActivity<TSource> next)
            : base(current, null)
        {
            _next = next;
        }

        public ActivityBuilder(IActivity current, IActivity next)
            : base(current, next)
        {
        }

        #endregion

        IActivity<TSource> IActivityBuilder<TActivity, TSource>.Build()
        {
            if (_current != null && _next != null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is presented", "ActivityBuilder{T1,T2}");
                return new Activity<TSource>(_current, _next);
            }
            else if (_current != null && _next == null)
            {
                Debug.WriteLine("[Current] is presented AND [Next] is not presented", "ActivityBuilder{T1,T2}");
                return new Activity<TSource>(_current, base._next);
            }
            else if (_current == null && _next != null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is presented", "ActivityBuilder{T1,T2}");
                return new Activity<TSource>(base._current, _next);
            }
            else if (_current == null && _next == null)
            {
                Debug.WriteLine("[Current] is not presented AND [Next] is not presented", "ActivityBuilder{T1,T2}");
                return new Activity<TSource>(base._current, base._next);
            }

            throw new NotImplementedException("There is no suitable build methods.");
        }

        public IActivityBuilder<TNextActivity, TSource, TNewResult, TNewResult> Then<TNextActivity, TNewResult>(
            Action<TNextActivity> builder,
            Func<TSource, TNewResult> selector)
            where TNextActivity : IActivity<TSource, TNewResult>, new()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Activity builder for activities uses no source and generates no result
    /// </summary>
    public class ActivityBuilder : IActivityBuilder
    {
        protected readonly IActivity _current;
        protected readonly IActivity _next;

        public ActivityBuilder(IActivity current, IActivity next)
        {
            _current = current;
            _next = next;
        }

        public IActivity Build()
        {
            Debug.WriteLine("Executing build...", "ActivityBuilder");
            return new Activity(_current, _next);
        }

        public IActivityBuilder Then<TActivity>(
            Action<TActivity> builder)
            where TActivity : IActivity, new()
        {
            var current = new Activity(_current, _next);
            var next = builder.SetupActivity();
            return new ActivityBuilder(current, next);
        }

        public IActivityBuilder<TActivity, TSource> Then<TActivity, TSource>(
            Action<TActivity> builder)
            where TActivity : IActivity<TSource>, new()
        {
            var current = new Activity(_current, _next);
            var next = builder.SetupActivity();
            return new ActivityBuilder<TActivity, TSource>(current, next);
        }

        public IActivityBuilder<TActivity, TSource, TResult, TResult> Then<TActivity, TSource, TResult>(
            Action<TActivity> builder,
            Func<TSource, TResult> selector)
            where TActivity : IActivity<TSource, TResult>, new()
        {
            var current = new Activity(_current, _next);
            var next = builder.SetupActivity();
            return new ActivityBuilder<TActivity, TSource, TResult, TResult>(current, next);
        }

        public IActivityBuilder<TActivity, TSource, TSourceResult, TNextResult> Then<TActivity, TSource, TSourceResult, TNextResult>(
            Action<TActivity> builder,
            Func<TSource, TNextResult> selector)
            where TActivity : IActivity<TSource, TNextResult>, new()
        {
            var current = new Activity(_current, _next);
            var next = builder.SetupActivity();
            return new ActivityBuilder<TActivity, TSource, TSourceResult, TNextResult>(current, next);
        }
    }
}
