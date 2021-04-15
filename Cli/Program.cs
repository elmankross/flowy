using Engine;
using Engine.Activities;
using System;

namespace Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var workflow = new WorkflowBuilder()
                .When<Timer>(x => x.Interval = TimeSpan.FromSeconds(1))
                .Then<Variable<int>, int>(x => x.Value = new Random().Next(0, 100))
                .Then<HttpRequest<int>, string>(builder =>
                {
                    builder.HttpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
                    };
                    builder.Selector = (id, client) => client.GetStringAsync("todos/" + id);
                })
                .Then<WriteToConsole, string>(_ => { }, x => x.ToString());
        }
    }
}
