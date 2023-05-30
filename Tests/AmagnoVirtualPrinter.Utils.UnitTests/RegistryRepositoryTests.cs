using NUnit.Framework;

namespace AmagnoVirtualPrinter.Utils.UnitTests
{
    [TestFixture]
    public class RegistryRepositoryTests
    {
        [Test]
        public void TryGetGhostscriptPath_FindsGhostScriptPath()
        {
            var sut = new RegistryRepository();

            var result = sut.TryGetGhostscriptPath(out var path);

            Assert.True(result);
            Assert.IsNotNull(path);
        }
    }
}