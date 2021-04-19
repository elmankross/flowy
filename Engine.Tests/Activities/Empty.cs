using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Tests.Activities
{
    public class Empty : IActivity
    {
        public bool Processed { get; private set; } = false;

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
        {
            Processed = true;
            return next();
        }
    }
}
