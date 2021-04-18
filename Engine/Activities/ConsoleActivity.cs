using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    [DebuggerDisplay("Console")]
    [Metadata(Category = "System", Description = "Write message to console", Name = "Console")]
    public class ConsoleActivity : IActivity<string>
    {
        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "WriteToConsole");
            return ExecuteAsync("<no text>", next, token);
        }

        public Task ExecuteAsync(string source, Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "WriteToConsole{T}");
            Console.WriteLine(source);
            return next();
        }
    }
}