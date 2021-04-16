using Engine;
using Engine.Activities;
using System;
using System.Threading.Tasks;

namespace Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var workflow = new WorkflowBuilder()
                .When<Timer>(x => x.Interval = TimeSpan.FromSeconds(15))
                .Then<HttpRequest<It.NothingToAccept, string>, It.NothingToAccept, string>(builder =>
                {
                    builder.HttpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
                    };
                    var id = new Random().Next(0, 100);
                    builder.Selector = (_, client) => client.GetStringAsync("todos/" + id);
                }, x => string.Empty)
                .Then<WriteToConsole, string>(_ => { })
                .Build();

            await workflow.ExecuteAsync(default, () => Task.CompletedTask);

            while (true)
            {
                await Task.Delay(200);
            }
        }
    }
}
