using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Activities
{
    public class ExecuteHttpRequest<TAccept, TReturn> : IActivity<TAccept, TReturn>
    {
        public IActivityMeta Meta { get; } = new ActivityMeta
        {
            [ActivityMeta.Key.Name] = "HttpRequest",
            [ActivityMeta.Key.Description] = "Executes http request with result or without it"
        };

        public Func<HttpClient> HttpClientFactory { private get; set; }
        public Func<TAccept, HttpClient, Task<TReturn>> Action { private get; set; }

        public async Task ExecuteAsync(TAccept accept, Func<TReturn, Task> next, CancellationToken token)
        {
            var client = HttpClientFactory();
            var result = await Action(accept, client);
            await next(result);
        }

        public Task ExecuteAsync(TAccept accept, Func<Task> next, CancellationToken token)
            => ExecuteAsync(accept, _ => next(), token);

        public Task ExecuteAsync(Func<Task> next, CancellationToken token)
            => ExecuteAsync(default, next, token);
    }
}
