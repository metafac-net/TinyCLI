using MetaFac.TinyCLI;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI.Tests
{
    internal class TestCommands : CommandsBase
    {
        private ValueTask<int> Command0(OtherArgs otherArgs)
        {
            return new ValueTask<int>(0);
        }
        public TestCommands(CmdOptions options) : base("test", "Test commands", options)
        {
            AddCommand<int>("cmd0", "Command zero", Command0);
        }
    }
}