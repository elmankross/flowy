using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class HttpRequest<TSource, TResult> : IActivity<TSource, TResult>
    {
        public HttpClient HttpClient { get; set; }
        public Func<TSource, HttpClient, Task<TResult>> Selector { get; set; }

        public async Task ExecuteAsync(Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "HttpRequest");
            await ExecuteAsync(default, next, token);
        }

        public async Task ExecuteAsync(TSource source, Func<Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "HttpRequest{T}");
            await ExecuteAsync(source, _ => next(), token);
        }

        public async Task ExecuteAsync(TSource source, Func<TResult, Task> next, CancellationToken token = default)
        {
            Debug.WriteLine("Executed async.", "HttpRequest{T1,T2}");
            var result = await Selector(source, HttpClient);
            await next(result);
        }
    }
}
