using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Tools.Logging.Tests; 

[TestFixture]
public class LoggerTests {
    [SetUp]
    public void Setup() {
        LoggerFactory.Reset();
    }

    [Test]
    public void TraceLevelDefault() {
        Logger log = LoggerFactory.GetLogger("Pepe");
        Assert.That(log.MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
    }

    [Test]
    public void LoggersAreCachedCaseInsensitive() {
        Assert.That(LoggerFactory.Loggers.Count, Is.EqualTo(0));
        Logger log = LoggerFactory.GetLogger("Pepe");
        Assert.That(log.MaxTraceLevel, Is.EqualTo(TraceLevel.Error));

        Assert.That(LoggerFactory.Loggers.Count, Is.EqualTo(1));
        Logger log2 = LoggerFactory.GetLogger("PEPE");

        Assert.That(log.GetHashCode(), Is.EqualTo(log2.GetHashCode()));
        Assert.That(LoggerFactory.Loggers.Count, Is.EqualTo(1));
    }

    [Test]
    public void TraceLevelTests() {
        Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
        Assert.That(LoggerFactory.GetLogger("PLAYER").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));

        LoggerFactory.SetTraceLevel("PLAYER", TraceLevel.Debug);
        Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
        Assert.That(LoggerFactory.GetLogger("PLAYER").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));

        LoggerFactory.SetDefaultTraceLevel(TraceLevel.Fatal);
        Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Fatal));
        Assert.That(LoggerFactory.GetLogger("PLAYER").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));

        LoggerFactory.OverrideTraceLevel(TraceLevel.Info);
        Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
        Assert.That(LoggerFactory.GetLogger("PLAYER").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));

        LoggerFactory.SetDefaultTraceLevel(TraceLevel.Fatal);
        Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Fatal));
        Assert.That(LoggerFactory.GetLogger("PLAYER").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
    }
}