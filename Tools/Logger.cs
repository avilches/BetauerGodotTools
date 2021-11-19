using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using NUnit.Framework;
using File = System.IO.File;

namespace Tools {
    public enum TraceLevel {
        Off = 0,
        Fatal = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
        Debug = 5,
    }

    internal class TraceLevelConfig {
        internal string Type { get; }
        internal string Name { get; }
        internal TraceLevel TraceLevel;

        public TraceLevelConfig(string type, string name, TraceLevel traceLevel) {
            Type = type.ToLower();
            Name = name != null ? name.ToLower() : "";
            TraceLevel = traceLevel;
        }

        public bool Equals(string type, string name) {
            return type.ToLower().Equals(Type) && (name != null ? name.ToLower() : "").Equals(Name);
        }
    }

    public class LoggerFactory : Node {
        internal static LoggerFactory Instance { get; } = new LoggerFactory();
        internal static ITextWriter[] Writers => Instance._writers;

        internal readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
        private readonly List<TraceLevelConfig> _traceLevelConfig = new List<TraceLevelConfig>();
        private ITextWriter[] _writers = { };
        private int _frame = -1;
        private TraceLevelConfig _defaultTraceLevelConfig = new TraceLevelConfig("<default>", "<default>", TraceLevel.Info);

        private LoggerFactory() {
        }

        public static int Frame => Instance._frame;

        public static void Reset() {
            Instance._loggers.Clear();
            Instance._traceLevelConfig.Clear();
            Instance._frame = -1;
            Instance._writers = Array.Empty<ITextWriter>();
            Instance._defaultTraceLevelConfig = new TraceLevelConfig("<default>", "<default>", TraceLevel.Info);
        }
        public static LoggerFactory AddConsoleOut() {
            return AddTextWriter(Console.Out);
        }

        public static LoggerFactory AddFileWriter(string logPath) {
            return AddTextWriter(File.CreateText(logPath));
        }

        public static LoggerFactory AddGodotPrinter() {
            return Instance.AddTextWriter(new GodotPrinter());
        }

        public static LoggerFactory AddTextWriter(TextWriter textWriter) {
            return Instance.AddTextWriter(new TextWriterDelegate(textWriter));
        }

        public static LoggerFactory SetTraceLevel(Type type, TraceLevel traceLevel) {
            SetTraceLevel(ReflectionTools.GetTypeWithoutGenerics(type), "*", traceLevel);
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(Type type, string name, TraceLevel traceLevel) {
            SetTraceLevel(ReflectionTools.GetTypeWithoutGenerics(type), name, traceLevel);
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(string type, TraceLevel traceLevel) {
            SetTraceLevel(type, "*", traceLevel);
            return Instance;
        }

        private static string WildCardToRegular(string value) {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            // If you want to implement "*" only
            // return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }

        public static LoggerFactory SetDefaultTraceLevel(TraceLevel traceLevel) {
            Instance._defaultTraceLevelConfig.TraceLevel = traceLevel;
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(string type, string name, TraceLevel traceLevel) {
            name ??= "*";
            TraceLevelConfig traceLevelConfig =
                Instance._traceLevelConfig.FirstOrDefault(tlc => tlc.Equals(type, name));
            if (traceLevelConfig != null) {
                traceLevelConfig.TraceLevel = traceLevel;
            } else {
                Instance._traceLevelConfig.Add(new TraceLevelConfig(type, name, traceLevel));
            }
            RefreshTraceLevelLoggers();
            return Instance;
        }

        private static void RefreshTraceLevelLoggers() {
            foreach (var logger in Instance._loggers.Values) {
                TraceLevelConfig traceLevelConfig = FindTraceLevelConfig(logger.Type, logger.Name);
                if (traceLevelConfig != null) {
                    logger.TraceLevelConfig = traceLevelConfig;
                }
            }
        }

        private LoggerFactory AddTextWriter(ITextWriter textWriter) {
            ITextWriter[] writers = new ITextWriter[_writers.Length + 1];
            _writers.CopyTo(writers, 0);
            writers[_writers.Length] = textWriter;
            _writers = writers;
            return this;
        }

        public sealed override void _PhysicsProcess(float delta) {
            _frame = (_frame + 1) % 10000; // D4 format: 0000-9999 (10000/60/60, up to 2.7)
        }

        public static Logger GetLogger(Type type, string name = null) {
            return GetLogger(type.Name, name);
        }

        public static Logger GetLogger(string type, string name = null) {
            var nameKey = _CreateLoggerKey(type, name);
            Instance._loggers.TryGetValue(nameKey, out Logger logger);
            return logger ?? _CreateNewLogger(type, name, nameKey);
        }

        private static Logger _CreateNewLogger(string type, string name, string nameKey) {
            TraceLevelConfig traceLevelConfig = FindTraceLevelConfig(type, name) ?? Instance._defaultTraceLevelConfig;
            Logger logger = new Logger(type, name, traceLevelConfig);
            Instance._loggers.Add(nameKey, logger);
            return logger;
        }

        private static string _CreateLoggerKey(string type, string name) {
            string result = type;
            if (name != null) {
                result += $".{name}";
            }
            return result.ToLower();
        }

        private static TraceLevelConfig FindTraceLevelConfig(string type, string name) {
            type =  type.ToLower();
            name = name == null ? "" : name.ToLower();

            int maxScore = 0;
            TraceLevelConfig result = null;
            foreach (TraceLevelConfig levelConfig in Instance._traceLevelConfig) {
                int score = 0;
                if (type.Equals(levelConfig.Type)) {
                    score += 10000000;
                } else if (Regex.IsMatch(type, WildCardToRegular(levelConfig.Type))) {
                    score += levelConfig.Type.Length * 1000;
                }
                if (score > 0) {
                    if (name.Equals(levelConfig.Name)) {
                        score += 100;
                    } else if (Regex.IsMatch(name, WildCardToRegular(levelConfig.Name))) {
                        score += levelConfig.Name.Length;
                    } else {
                        score = 0;
                    }
                    if (score > maxScore) {
                        maxScore = score;
                        result = levelConfig;

                    }
                }
            }
            return result;
        }

        public static void Dispose() {
            foreach (ITextWriter writer in Instance._writers) writer.Dispose();
        }

        public static void Start(Node node) {
            node.AddChild(Instance);
        }
    }

    public class Logger {
        public readonly string TimeFormat;
        public readonly string TraceFormat;

        internal TraceLevelConfig TraceLevelConfig { get; set; }
        public TraceLevel MaxTraceLevel => TraceLevelConfig.TraceLevel;
        public bool Enabled { get; set; } = true;

        public readonly string Type;
        public readonly string Name;
        private readonly string _title;

        public static int Frame => LoggerFactory.Frame;
        // public static float Delta => LoggerFactory.Delta;

        internal Logger(
            string type,
            string name,
            TraceLevelConfig traceLevelConfig,
            string timeFormat = "yyyy-M-dd HH:mm:ss.fff",
            string traceFormat = "{0} [{1,4}] {2,5} {3} {4}") {
            Type = type;
            Name = name;
            _title = _CreateLoggerString(type, name);
            TraceLevelConfig = traceLevelConfig;
            TimeFormat = timeFormat;
            TraceFormat = traceFormat;
        }

        private static string _CreateLoggerString(string type, string name) {
            string result = $"[{type}]";
            ;
            if (name != null) {
                result += $" [{name}";
                result += "]";
            }
            return result;
        }


        public void Fatal(string message) => Log(TraceLevel.Fatal, message);
        public void Error(string message) => Log(TraceLevel.Error, message);
        public void Warning(string message) => Log(TraceLevel.Warning, message);
        public void Info(string message) => Log(TraceLevel.Info, message);
        public void Debug(string message) => Log(TraceLevel.Debug, message);

        public void Fatal(string message, params object[] args) => Log(TraceLevel.Fatal, message, args);
        public void Error(string message, params object[] args) => Log(TraceLevel.Error, message, args);
        public void Warning(string message, params object[] args) => Log(TraceLevel.Warning, message, args);
        public void Info(string message, params object[] args) => Log(TraceLevel.Info, message, args);
        public void Debug(string message, params object[] args) => Log(TraceLevel.Debug, message, args);

        private void Log(TraceLevel level, string format, params object[] args) =>
            Log(level, string.Format(format, args));

        private void Log(TraceLevel level, string message) {
            if (!Enabled || level > MaxTraceLevel) return;
            var line = string.Format(TraceFormat,
                DateTime.Now.ToString(TimeFormat),
                Frame,
                level.ToString(),
                _title,
                message);
            foreach (ITextWriter writer in LoggerFactory.Writers) {
                writer.WriteLine(line);
                writer.Flush();
            }
        }
    }

    public interface ITextWriter {
        void WriteLine(string line);
        void Flush();
        void Dispose();
    }

    public class TextWriterDelegate : ITextWriter {
        private TextWriter _delegate;

        public TextWriterDelegate(TextWriter @delegate) {
            _delegate = @delegate;
        }

        public void WriteLine(string line) {
            _delegate.WriteLine(line);
        }

        public void Flush() {
            _delegate.Flush();
        }

        public void Dispose() {
            _delegate.Dispose();
        }
    }

    public class GodotPrinter : ITextWriter {
        public void WriteLine(string line) {
            GD.Print(line);
        }

        public void Flush() {
        }

        public void Dispose() {
        }
    }

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
            Assert.That(LoggerFactory.Instance._loggers.Count, Is.EqualTo(0));
            Logger log = LoggerFactory.GetLogger("Pepe");
            Assert.That(log.MaxTraceLevel, Is.EqualTo(TraceLevel.Info));

            Assert.That(LoggerFactory.Instance._loggers.Count, Is.EqualTo(1));
            Logger log2 = LoggerFactory.GetLogger("PEPE");

            Assert.That(log.GetHashCode(), Is.EqualTo(log2.GetHashCode()));
            Assert.That(LoggerFactory.Instance._loggers.Count, Is.EqualTo(1));

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
    }

}