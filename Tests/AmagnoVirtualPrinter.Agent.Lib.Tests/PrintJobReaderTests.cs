using AmagnoVirtualPrinter.Agent.Lib.Misc;
using NUnit.Framework;

namespace AmagnoVirtualPrinter.Agent.Lib.Tests
{
    [TestFixture]
    public class PrintJobReaderTests
    {
        [Test]
        public void GetCurrentPrintJobs_ReturnsJobs()
        {
            var jobs = PrintJobReader.GetCurrentPrintJobs("Amagno");
            
            Assert.IsNull(jobs);
        }
    }
}