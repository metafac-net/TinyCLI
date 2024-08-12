using FluentAssertions;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using PublicApiGenerator;

namespace MetaFac.TinyCLI.Tests
{
    public class PublicApiRegressionTests
    {
        [Fact]
        public void VersionCheck()
        {
            ThisAssembly.AssemblyVersion.Should().Be("1.9.0.0");
        }

        [Fact]
        public async Task CheckPublicApi()
        {
            // act
            var options = new ApiGeneratorOptions()
            {
                IncludeAssemblyAttributes = false
            };
            string currentApi = ApiGenerator.GeneratePublicApi(typeof(CommandsBase).Assembly, options);

            // assert
            await Verifier.Verify(currentApi);
        }

    }
}