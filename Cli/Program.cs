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
                .When<Timer>(x => x.Interval = TimeSpan.FromSeconds(5))
                .Then<Variable<int>, int>(x => x.Value = new Random().Next(0, 100))
                .Then<HttpRequest<int, string>, string>(builder =>
                {
                    builder.HttpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
                    };
                    builder.Selector = (id, client) => client.GetStringAsync("todos/" + id);
                }, x => x.ToString())
                .Then<WriteToConsole>(_ => { })
                .Build();

            await workflow.ExecuteAsync(() => Task.CompletedTask);

            while (true)
            {
                await Task.Delay(200);
            }
        }
    }
}
