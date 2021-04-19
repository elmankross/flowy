using Engine.Tests.Activities;
using System.Threading.Tasks;
using Xunit;

namespace Engine.Tests
{
    public class GroupTests
    {
        [Fact]
        public async Task Group__CurrentWithoutInputAndNextWithoutInput__ShouldBeProcessed()
        {
            var current = new Empty();
            var next = new Empty();
            var group = new ActivityGroup(current, next);

            await group.ExecuteAsync(() => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.True(current.Processed);
            Assert.True(next.Processed);
        }

        [Fact]
        public async Task Group__CurrentWithInputAndNextWithoutInput__ShouldBeProcessed()
        {
            var input = 4;
            var current = new Accepts<int>();
            var next = new Empty();
            var group = new ActivityGroup<int>(current, next);

            await group.ExecuteAsync(input, () => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.Equal(input, current.Value);
            Assert.True(next.Processed);
        }

        [Fact]
        public async Task Group__CurrentWithOutputAndNextWithInput__ShouldBeProcessed()
        {
            var input = 4;
            var current = new Returns<int, int> { Selector = x => x };
            var next = new Accepts<int>();
            var group = new ActivityGroup<int>(current, next);

            await group.ExecuteAsync(input, () => Task.CompletedTask, new System.Threading.CancellationToken());

            Assert.Equal(input, next.Value);
        }

        [Fact]
        public async Task Group__CurrentWithOutputAndNextWithOutput__ShouldBeProcessed()
        {
            var input = 4;
            var output = string.Empty;
            var group = new ActivityGroup<int, int, string>(
                new Returns<int, int> { Selector = x => x },
                new Returns<int, string> { Selector = x => x.ToString() });

            await group.ExecuteAsync(input, x =>
            {
                output = x;
                return Task.CompletedTask;
            }, new System.Threading.CancellationToken());

            Assert.Equal("4", output);
        }
    }
}
