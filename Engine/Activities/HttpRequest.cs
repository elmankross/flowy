using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class HttpRequest<TSource> : IActivity<TSource, string>
    {
        public HttpClient HttpClient { get; set; }
        public Func<TSource, HttpClient, Task<string>> Selector { get; set; }

        public Task ExecuteAsync(TSource source, Func<string, Task> next, CancellationToken token = default)
        {
            return Selector(source, HttpClient);
        }

        public Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            return next();
        }
    }
}
