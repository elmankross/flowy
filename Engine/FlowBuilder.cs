using System;

namespace Engine
{
    public static class FlowBuilder
    {
        /// <summary>
        /// ([X] -> [X])
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IActivityBuilder When<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity, new()
            => When(builder.Create());

        /// <summary>
        /// ([X] -> [X])
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IActivityBuilder When<TActivity>(TActivity current)
            where TActivity : IActivity, new()
            => new ActivityBuilder(current);

        /// <summary>
        /// (I -> [X])
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TAccept"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IActivityBuilder<TAccept> When<TActivity, TAccept>(Action<TActivity> builder = default)
            where TActivity : IActivity<TAccept>, new()
            => When<TActivity, TAccept>(builder.Create());

        /// <summary>
        /// (I -> [X])
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TAccept"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IActivityBuilder<TAccept> When<TActivity, TAccept>(TActivity current)
            where TActivity : IActivity<TAccept>, new()
            => new ActivityBuilder<TAccept>(current);

        /// <summary>
        /// (I -> O)
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TAccept"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IActivityBuilder<TAccept, TReturn> When<TActivity, TAccept, TReturn>(Action<TActivity> builder = default)
            where TActivity : IActivity<TAccept, TReturn>, new()
            => When<TActivity, TAccept, TReturn>(builder.Create());

        /// <summary>
        /// (I -> O)
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TAccept"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IActivityBuilder<TAccept, TReturn> When<TActivity, TAccept, TReturn>(TActivity current)
              where TActivity : IActivity<TAccept, TReturn>, new()
            => new ActivityBuilder<TAccept, TReturn>(current);
    }

    public interface IActivityBuilder
    {
        IActivity Build();

        /// <summary>
        /// ([X] -> [X], [X] -> [X]) with builder
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity, new();

        /// <summary>
        /// ([X] -> [X], [X] -> [X]) with activity
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        IActivityBuilder Then<TActivity>(TActivity next)
            where TActivity : IActivity, new();
    }

    public interface IActivityBuilder<TAccept> : IActivityBuilder
    {
        new IActivity<TAccept> Build();

        /// <summary>
        /// (I -> [X], [X] -> [X]) with builder
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        new IActivityBuilder<TAccept> Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity, new();

        /// <summary>
        /// (I -> [X], [X] -> [X]) with instance
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        new IActivityBuilder<TAccept> Then<TActivity>(TActivity next)
           where TActivity : IActivity, new();
    }

    public interface IActivityBuilder<TAccept, TReturn> : IActivityBuilder<TAccept>
    {
        new IActivity<TAccept, TReturn> Build();

        /// <summary>
        /// (I -> O, O -> [X]) with builder
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        new IActivityBuilder<TAccept> Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity<TAccept>, new();

        /// <summary>
        /// (I -> O, O -> [X]) with instance
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <returns></returns>
        new IActivityBuilder<TAccept> Then<TActivity>(TActivity next)
            where TActivity : IActivity<TAccept>, new();

        /// <summary>
        /// (I -> O1, O1 -> O2) with builder
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TNextReturn"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        IActivityBuilder<TAccept, TNextReturn> Then<TActivity, TNextReturn>(Action<TActivity> builder = default)
            where TActivity : IActivity<TReturn, TNextReturn>, new();

        /// <summary>
        /// (I -> O1, O1 -> O2) with instance
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TNextReturn"></typeparam>
        /// <returns></returns>
        IActivityBuilder<TAccept, TNextReturn> Then<TActivity, TNextReturn>(TActivity next)
            where TActivity : IActivity<TReturn, TNextReturn>, new();
    }

    public class ActivityBuilder : IActivityBuilder
    {
        private readonly IActivity _current;

        public ActivityBuilder(IActivity current)
        {
            _current = current;
        }

        public IActivity Build() => _current;

        public IActivityBuilder Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity, new()
            => Then(builder.Create());

        public IActivityBuilder Then<TActivity>(TActivity next)
            where TActivity : IActivity, new()
        {
            var group = new ActivityGroup(_current, next);
            return new ActivityBuilder(group);
        }
    }

    public class ActivityBuilder<TAccept> : IActivityBuilder<TAccept>
    {
        private readonly IActivity<TAccept> _current;

        public ActivityBuilder(IActivity<TAccept> current)
        {
            _current = current;
        }

        public IActivity<TAccept> Build() => _current;

        public IActivityBuilder<TAccept> Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity, new()
            => Then(builder.Create());

        public IActivityBuilder<TAccept> Then<TActivity>(TActivity next)
            where TActivity : IActivity, new()
        {
            var group = new ActivityGroup<TAccept>(_current, next);
            return new ActivityBuilder<TAccept>(group);
        }

        IActivity IActivityBuilder.Build() => Build();
        IActivityBuilder IActivityBuilder.Then<TActivity>(Action<TActivity> builder) => Then(builder);
        IActivityBuilder IActivityBuilder.Then<TActivity>(TActivity next) => Then(next);
    }

    public class ActivityBuilder<TAccept, TReturn> : IActivityBuilder<TAccept, TReturn>
    {
        private readonly IActivity<TAccept, TReturn> _current;

        public ActivityBuilder(IActivity<TAccept, TReturn> current)
        {
            _current = current;
        }

        public IActivity<TAccept, TReturn> Build() => _current;

        public IActivityBuilder<TAccept> Then<TActivity>(Action<TActivity> builder = default)
            where TActivity : IActivity<TAccept>, new()
            => Then(builder.Create());

        public IActivityBuilder<TAccept> Then<TActivity>(TActivity next)
            where TActivity : IActivity<TAccept>, new()
        {
            var group = new ActivityGroup<TAccept>((IActivity<TAccept, TAccept>)_current, next);
            return new ActivityBuilder<TAccept>(group);
        }

        public IActivityBuilder<TAccept, TNextReturn> Then<TActivity, TNextReturn>(Action<TActivity> builder = default)
            where TActivity : IActivity<TReturn, TNextReturn>, new()
            => Then<TActivity, TNextReturn>(builder.Create());

        public IActivityBuilder<TAccept, TNextReturn> Then<TActivity, TNextReturn>(TActivity next)
            where TActivity : IActivity<TReturn, TNextReturn>, new()
        {
            var group = new ActivityGroup<TAccept, TReturn, TNextReturn>(_current, next);
            return new ActivityBuilder<TAccept, TNextReturn>(group);
        }

        IActivity IActivityBuilder.Build() => Build();
        IActivity<TAccept> IActivityBuilder<TAccept>.Build() => Build();
        IActivityBuilder IActivityBuilder.Then<TActivity>(Action<TActivity> builder)
        {
            var next = builder.Create();
            var group = new ActivityGroup(_current, next);
            return new ActivityBuilder(group);
        }

        IActivityBuilder<TAccept> IActivityBuilder<TAccept>.Then<TActivity>(Action<TActivity> builder)
        {
            var next = builder.Create();
            var group = new ActivityGroup<TAccept>(_current, next);
            return new ActivityBuilder<TAccept>(group);
        }

        IActivityBuilder<TAccept, TNextReturn> IActivityBuilder<TAccept, TReturn>.Then<TActivity, TNextReturn>(Action<TActivity> builder)
        {
            var next = builder.Create();
            var group = new ActivityGroup<TAccept, TReturn, TNextReturn>(_current, next);
            return new ActivityBuilder<TAccept, TNextReturn>(group);
        }

        IActivityBuilder<TAccept> IActivityBuilder<TAccept>.Then<TActivity>(TActivity next)
        {
            throw new NotImplementedException();
        }

        IActivityBuilder IActivityBuilder.Then<TActivity>(TActivity next)
        {
            throw new NotImplementedException();
        }
    }
}
