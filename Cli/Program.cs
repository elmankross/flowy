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
                .When<TimerActivity>(x => x.Interval = TimeSpan.FromSeconds(600))
                .Then<HttpRequestActivity<It.NothingToAccept, string>, It.NothingToAccept, string>(builder =>
                {
                    builder.HttpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
                    };
                    var id = new Random().Next(0, 100);
                    builder.Selector = (_, client) => client.GetStringAsync("todos/" + id);
                })
                .Then<ConsoleActivity, string>()
                .Then<ConverterActivity<string, string>, string, string>(builder =>
                {
                    builder.Selector = (_) => _;
                })
                .Build();

            await workflow.ExecuteAsync("aaa", x => Task.CompletedTask);

            while (true)
            {
                await Task.Delay(200);
            }
        }
    }
}
