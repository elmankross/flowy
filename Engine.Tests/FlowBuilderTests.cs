using Engine.Tests.Activities;
using System.Threading.Tasks;
using Xunit;

namespace Engine.Tests
{
    public class FlowBuilderTests
    {
        /// <summary>
        /// ([X] -> [X], [X] -> [X])
        /// </summary>
        [Fact]
        public async Task FlowBuilder__WithNoInputAndNoOutput__ShouldBeProcessed()
        {
            var input = new Empty();
            var output = new Empty();
            var flow = FlowBuilder
                .When(input)
                .Then(output)
                .Build();

            await flow.ExecuteAsync(() => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.True(input.Processed);
            Assert.True(output.Processed);
        }

        /// <summary>
        /// (I -> [X], [X] -> [X])
        /// </summary>
        [Fact]
        public async Task FlowBuilder__WithInputAndNoOutput__FirstPart__ShouldBeProcessed()
        {
            var inputText = "Hello world!";
            var input = new Accepts<string>();
            var output = new Empty();
            var flow = FlowBuilder
                .When<Accepts<string>, string>(input)
                .Then(output)
                .Build();

            await flow.ExecuteAsync(inputText, () => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.Equal(inputText, input.Value);
            Assert.True(output.Processed);
        }

        /// <summary>
        /// (I -> O, O -> [X])
        /// </summary>
        [Fact]
        public async Task FlowBuilder__WithInputAndNoOutput__SecondPart__ShouldBeProcessed()
        {
            var input = "Hello world!";
            var output = new Accepts<string>();
            var flow = FlowBuilder
                .When<Returns<string, string>, string, string>(x => x.Selector = _ => _)
                .Then(output)
                .Build();

            await flow.ExecuteAsync(input, () => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.Equal(input, output.Value);
        }

        /// <summary>
        /// (I -> O1, O1 -> O2)
        /// </summary>
        [Fact]
        public async Task FlowBuilder__WithInputAndOutput__ShouldBeProcessed()
        {
            var outputText = string.Empty;
            var input = new Returns<string, string> { Selector = _ => _ + "o" };
            var output = new Returns<string, string> { Selector = _ => _ + "w" };
            var flow = FlowBuilder
                .When<Returns<string, string>, string, string>(input)
                .Then<Returns<string, string>, string>(output)
                .Build();

            await flow.ExecuteAsync("Hell", result =>
            {
                outputText = result;
                return Task.CompletedTask;
            }, new System.Threading.CancellationToken());

            Assert.Equal("Hellow", outputText);
        }
    }
}
