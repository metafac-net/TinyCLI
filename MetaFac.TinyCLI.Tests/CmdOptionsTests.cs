using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace MetaFac.TinyCLI.Tests
{
    public class CmdOptionsTests
    {
        [Fact]
        public void CreateEmptyCommands()
        {
            var options = new CmdOptions();
            var commands = new TestCommands(options);
            commands.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, -1)]
        public async Task TestExtraArgsAllowed(bool allowExtraArguments, int expectedResult)
        {
            var options = new CmdOptions(allowExtraArguments: allowExtraArguments);
            var commands = new TestCommands(options);
            string[] args = { "cmd0", "-x", "value" };
            int result = await commands.Run(args);
            result.ShouldBe(expectedResult);
        }
    }
}