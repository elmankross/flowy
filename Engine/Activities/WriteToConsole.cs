using System;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    [Metadata(Category = "Outputes", Description = "Write message to console", Name = "WriteToConsole")]
    public class WriteToConsole : IActivity<string, string>
    {
        public Task ExecuteAsync(string source, Func<string, Task> next, CancellationToken token = default)
        {
            Console.WriteLine(source);
            return next(source);
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            return next();
        }
    }
}