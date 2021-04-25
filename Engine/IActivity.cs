using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Has no input and no output
    /// </summary>
    public interface IActivity
    {
        Task ExecuteAsync(Func<Task> next, CancellationToken token);
        IActivityMeta Meta { get; }
    }

    /// <summary>
    /// Has input and no output
    /// </summary>
    /// <typeparam name="TAccept"></typeparam>
    public interface IActivity<TAccept> : IActivity
    {
        Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token);
    }

    /// <summary>
    /// Has input and output
    /// </summary>
    /// <typeparam name="TAccept"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public interface IActivity<TAccept, TReturn> : IActivity<TAccept>
    {
        Task ExecuteAsync(TAccept accept, Func<TReturn, Task> next, CancellationToken token);
    }
}
