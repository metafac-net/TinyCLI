using FluentAssertions;
using Xunit;

namespace MiniCLI.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CreateEmptyCommands()
        {
            var commands = new TestCommands("test", "Test commands");
            commands.Should().NotBeNull();
        }
    }
}