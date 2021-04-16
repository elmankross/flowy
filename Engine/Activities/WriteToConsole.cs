using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    [Metadata(Category = "Outputes", Description = "Write message to console", Name = "WriteToConsole")]
    public class WriteToConsole : IActivity<string>
    {
        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "WriteToConsole");
            return ExecuteAsync(null, next, token);
        }

        public Task ExecuteAsync(string source, Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "WriteToConsole{T}");
            Console.WriteLine(source);
            return next();
        }
    }
}