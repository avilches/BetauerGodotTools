using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class LoggerTests {
        [SetUp]
        public void Setup() {
            LoggerFactory.Reset();
        }

        [Test]
        public void TraceLevelDefault() {
            Logger log = LoggerFactory.GetLogger("Pepe");
            Assert.That(log.MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
        }

        [Test]
        public void LoggersAreCachedCaseInsensitive() {
            Assert.That(LoggerFactory.Instance.Loggers.Count, Is.EqualTo(0));
            Logger log = LoggerFactory.GetLogger("Pepe");
            Assert.That(log.MaxTraceLevel, Is.EqualTo(TraceLevel.Info));

            Assert.That(LoggerFactory.Instance.Loggers.Count, Is.EqualTo(1));
            Logger log2 = LoggerFactory.GetLogger("PEPE");

            Assert.That(log.GetHashCode(), Is.EqualTo(log2.GetHashCode()));
            Assert.That(LoggerFactory.Instance.Loggers.Count, Is.EqualTo(1));
        }

        [Test]
        public void LoggerDefault() {
            LoggerFactory.SetTraceLevel("PLAYER", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // Exact match only applies
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Fatal);
            Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Fatal));
            Assert.That(LoggerFactory.GetLogger("H").MaxTraceLevel, Is.EqualTo(TraceLevel.Fatal));
        }

        [Test]
        public void Logger2() {
            LoggerFactory.SetTraceLevel("PLAYER", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("P", "X").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // Exact match only applies
            Assert.That(LoggerFactory.GetLogger("Player").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger3() {
            LoggerFactory.SetTraceLevel("PLAYER:*", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // Exact match only applies
            Assert.That(LoggerFactory.GetLogger("Player:").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("Player:xx", "X").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger4() {
            LoggerFactory.SetTraceLevel("PLAYER:*", "Jump", TraceLevel.Debug);
            // Type wrong + name ok
            Assert.That(LoggerFactory.GetLogger("P").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("P", "Jump").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player", "Jump").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // type ok + name wrong
            Assert.That(LoggerFactory.GetLogger("Player:xxxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "J").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "JumpX").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // type ok + name ok
            Assert.That(LoggerFactory.GetLogger("Player:", "Jump").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "Jump").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger5() {
            LoggerFactory.SetTraceLevel("PLAYER:*", "Jump:*", TraceLevel.Debug);
            // Type wrong + name ok
            Assert.That(LoggerFactory.GetLogger("P", "Jump:xxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player", "Jump:xxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // type ok + name wrong
            Assert.That(LoggerFactory.GetLogger("Player:xxxx").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "J").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "JumpX").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            // type ok + name ok
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "Jump:*").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger5_Cached() {
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "Jump:x").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            LoggerFactory.SetTraceLevel("PLAYER:*", "Jump:*", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "Jump:x").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            LoggerFactory.SetTraceLevel("PLAYER:*", "Jump:*", TraceLevel.Error);
            Assert.That(LoggerFactory.GetLogger("Player:xxxx", "Jump:x").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
        }

        [Test]
        public void Logger6() {
            LoggerFactory.SetTraceLevel("*", "*", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("x", "x").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("y", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger7() {
            LoggerFactory.SetTraceLevel("*", "*", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("PLAYER:*", "Jump:*", TraceLevel.Error);
            Assert.That(LoggerFactory.GetLogger("x", "x").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("y", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("player:X", "jump:x").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
        }

        [Test]
        public void Logger8() {
            LoggerFactory.SetTraceLevel("*", "*", TraceLevel.Debug);
            Assert.That(LoggerFactory.GetLogger("x", "x").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            LoggerFactory.SetTraceLevel("x*", "*", TraceLevel.Error);
            Assert.That(LoggerFactory.GetLogger("x", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            LoggerFactory.SetTraceLevel("x", "*", TraceLevel.Warning);
            Assert.That(LoggerFactory.GetLogger("x", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Warning));
        }

        [Test]
        public void Logger9() {
            LoggerFactory.SetTraceLevel("*", "*", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("a*", "*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("aa*", "*", TraceLevel.Warning);

            Assert.That(LoggerFactory.GetLogger("a").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            Assert.That(LoggerFactory.GetLogger("a", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            Assert.That(LoggerFactory.GetLogger("aa", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Warning));
            Assert.That(LoggerFactory.GetLogger("aaA", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Warning));
            Assert.That(LoggerFactory.GetLogger("aaAA", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Warning));
        }

        [Test]
        public void Logger10() {
            LoggerFactory.SetTraceLevel("*", "aa*", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("*", "a*", TraceLevel.Fatal);
            LoggerFactory.SetTraceLevel("a*", "*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("aa*", "*", TraceLevel.Warning);

            Assert.That(LoggerFactory.GetLogger("a", "aaa").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            Assert.That(LoggerFactory.GetLogger("a", "aa").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            Assert.That(LoggerFactory.GetLogger("a", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Error));
            Assert.That(LoggerFactory.GetLogger("aa", "y").MaxTraceLevel, Is.EqualTo(TraceLevel.Warning));
        }

        [Test]
        public void Logger11() {
            Assert.That(LoggerFactory.GetLogger("pepe").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("pepe", "a").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("pepe", "b").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));

            LoggerFactory.SetTraceLevel("pepe", TraceLevel.Debug);
            LoggerFactory.SetTraceLevel("pepe", "a", TraceLevel.Fatal);

            Assert.That(LoggerFactory.GetLogger("pepe").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("pepe", "a").MaxTraceLevel, Is.EqualTo(TraceLevel.Fatal));
            Assert.That(LoggerFactory.GetLogger("pepe", "b").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }

        [Test]
        public void Logger12() {
            Assert.That(LoggerFactory.GetLogger("pepe").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));
            Assert.That(LoggerFactory.GetLogger("pepe", "a").MaxTraceLevel, Is.EqualTo(TraceLevel.Info));

            // Second rule override the first one
            LoggerFactory.SetTraceLevel("pepe", "*", TraceLevel.Warning);
            LoggerFactory.SetTraceLevel("pepe", TraceLevel.Debug);

            Assert.That(LoggerFactory.GetLogger("pepe").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
            Assert.That(LoggerFactory.GetLogger("pepe", "b").MaxTraceLevel, Is.EqualTo(TraceLevel.Debug));
        }
    }}