using Engine;
using Engine.Activities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var data = new List<string>();
            var client = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
            };

            var flow = FlowBuilder
                .When<ExecuteHttpRequest<string, string>, string, string>(builder =>
                {
                    builder.HttpClientFactory = () => client;
                    builder.Action = (id, client) => client.GetStringAsync("posts/" + id);
                })
                .Then<WriteTo<string>, string>(builder =>
                {
                    builder.Action = d =>
                    {
                        data.Add(d);
                        return Task.CompletedTask;
                    };
                })
                .Then<ConvertTo<string, string>, string>(builder =>
                {
                    builder.Converter = value =>
                    {
                        var userId = JsonDocument.Parse(value).RootElement.GetProperty("userId").GetInt32().ToString();
                        return Task.FromResult(userId.ToString());
                    };
                })
                .Then<ExecuteHttpRequest<string, string>, string>(builder =>
                {
                    builder.HttpClientFactory = () => client;
                    builder.Action = (id, client) => client.GetStringAsync("users/" + id);
                })
                .Then<WriteTo<string>, string>(builder =>
                {
                    builder.Action = d =>
                    {
                        data.Add(d);
                        return Task.CompletedTask;
                    };
                })
                .Build();

            for (var i = 1; i <= 32; i *= 4)
            {
                await flow.ExecuteAsync(i.ToString(), _ => Task.CompletedTask, new System.Threading.CancellationToken());
                await Task.Delay(500);
            }

            foreach (var d in data)
            {
                Console.WriteLine(d);
            }
        }
    }
}
