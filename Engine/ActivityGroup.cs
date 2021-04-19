using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Group has no input and has no output
    /// </summary>
    public class ActivityGroup : IActivity
    {
        protected IActivity CurrentWithoutInput { get; }
        protected IActivity NextWithoutInput { get; }

        /// <summary>
        /// ([X] -> [X], [X] -> [X]) With no input and output
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        public ActivityGroup(IActivity current, IActivity next)
        {
            CurrentWithoutInput = current;
            NextWithoutInput = next;
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => CurrentWithoutInput.ExecuteAsync(() => NextWithoutInput.ExecuteAsync(next, token), token);
    }

    /// <summary>
    /// Group has input and has no output
    /// </summary>
    /// <typeparam name="TAccept"></typeparam>
    public class ActivityGroup<TAccept> : ActivityGroup, IActivity<TAccept>
    {
        protected IActivity<TAccept> CurrentWithoutOutput { get; }
        protected IActivity<TAccept, TAccept> CurrentWithOutput { get; }
        protected IActivity<TAccept> NextWithInput { get; }

        /// <summary>
        /// (I -> [X], [X] -> [X]) Accepts first and not returned after
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        public ActivityGroup(IActivity<TAccept> current, IActivity next)
            : base(null, next)
        {
            CurrentWithoutOutput = current;
        }

        /// <summary>
        /// (I -> I, I -> [X]) Accepts and pass this through to next stage that has no output
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        public ActivityGroup(IActivity<TAccept, TAccept> current, IActivity<TAccept> next)
            : base(null, null)
        {
            CurrentWithOutput = current;
            NextWithInput = next;
        }

        public Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token)
        {
            if (CurrentWithoutOutput != null)
            {
                return CurrentWithoutOutput.ExecuteAsync(accept, () => NextWithoutInput.ExecuteAsync(next, token), token);
            }

            if(NextWithInput != null)
            {
                return CurrentWithOutput.ExecuteAsync(accept, result => NextWithInput.ExecuteAsync(result, next, token), token);
            }

            throw new NotImplementedException(
                "You need to use 'ExecuteAsync' implementation that returns value, such as 'ExecuteAsync(accept, returns => {})'");
        }
    }

    /// <summary>
    /// Group has input and has output
    /// </summary>
    /// <typeparam name="TAccept"></typeparam>
    /// <typeparam name="TReturnCurrent"></typeparam>
    /// <typeparam name="TReturnNext"></typeparam>
    public class ActivityGroup<TAccept, TReturnCurrent, TReturnNext> : ActivityGroup<TAccept>, IActivity<TAccept, TReturnNext>
    {
        new protected IActivity<TAccept, TReturnCurrent> CurrentWithOutput { get; }
        protected IActivity<TReturnCurrent, TReturnNext> NextWithOutput { get; }

        /// <summary>
        /// (I -> O1, O1 -> O2) Accepts input, convert it to another type and returns
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        public ActivityGroup(
            IActivity<TAccept, TReturnCurrent> current,
            IActivity<TReturnCurrent, TReturnNext> next)
            : base(null, null)
        {
            CurrentWithOutput = current;
            NextWithOutput = next;
        }

        public Task ExecuteAsync(TAccept accept, Func<TReturnNext, Task> next, CancellationToken token)
            => CurrentWithOutput.ExecuteAsync(accept, result => NextWithOutput.ExecuteAsync(result, next, token), token);
    }
}
