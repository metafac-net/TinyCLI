using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using PublicApiGenerator;
using Shouldly;

namespace MetaFac.TinyCLI.Tests
{
    public class PublicApiRegressionTests
    {
        [Fact]
        public void VersionCheck()
        {
            ThisAssembly.AssemblyVersion.ShouldBe("2.0.0.0");
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