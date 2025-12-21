using CosmosCasino.Core.Debug.Environment;
using NUnit.Framework;

namespace CosmosCasino.Tests.Debug.Environment
{
    [TestFixture]
    internal class DebugCapabilitiesTests
    {
        [Test]
        public void DebugCapabilities_Are_Derived_From_Environment()
        {
#if DEBUG
            Assert.That(DebugCapabilities.CanViewDebugOverlay, Is.True);
            Assert.That(DebugCapabilities.CanViewVerboseLogs, Is.True);
            Assert.That(DebugCapabilities.CanExecuteUnsafeCommands, Is.True);
            Assert.That(DebugCapabilities.CanExecuteSafeCommands, Is.True);
#else
            Assert.That(DebugCapabilities.CanViewDebugOverlay, Is.True);
            Assert.That(DebugCapabilities.CanViewVerboseLogs, Is.False);
            Assert.That(DebugCapabilities.CanExecuteUnsafeCommands, Is.False);
            Assert.That(DebugCapabilities.CanExecuteSafeCommands, Is.True);
#endif
        }
    }
}
