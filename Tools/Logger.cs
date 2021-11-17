using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using File = System.IO.File;

namespace Tools {
    public enum TraceLevel {
        Error = 2,
        Warning = 3,
        Info = 4,
        Debug = 5,
        Off = 6,
    }

    public class LoggerFactory : Node {
        public static LoggerFactory Instance { get; } = new LoggerFactory();

        public static int Frame => Instance._frame;

        public static ITextWriter[] Writers => Instance._writers;

        private int _frame = -1;

        private ITextWriter[] _writers = { };
        private Dictionary<string, TraceLevel> _traceLevelConfig = new Dictionary<string, TraceLevel>();

        private LoggerFactory() {
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
            SetTraceLevel(LoggerName(type), traceLevel);
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(Type type, string name, TraceLevel traceLevel) {
            SetTraceLevel(LoggerName(type, name), traceLevel);
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(string type, string name, TraceLevel traceLevel) {
            SetTraceLevel(LoggerName(type, name), traceLevel);
            return Instance;
        }

        public static LoggerFactory SetTraceLevel(string name, TraceLevel traceLevel) {
            Instance._traceLevelConfig.Remove(name.ToLowerInvariant());
            Instance._traceLevelConfig.Add(name.ToLowerInvariant(), traceLevel);
            return Instance;
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

        public static string LoggerName(Type type, string name = null, int hashCode = 0) {
            return LoggerName(type.Name, name, hashCode);
        }

        public static string LoggerName(string type, string name, int hashCode = 0) {
            string result = type;
            if (name != null) {
                result += $".{name}";
            }
            if (hashCode > 0) {
                result += $".{hashCode:x8}";
            }
            return result;
        }

        public static Logger GetLogger(Type type) {
            return GetLogger(LoggerName(type));
        }

        public static Logger GetLogger(Type type, string name, int hashCode = 0) {
            return GetLogger(LoggerName(type, name, hashCode));
        }

        public static Logger GetLogger(string type, string name) {
            return GetLogger(LoggerName(type, name));
        }

        public static Logger GetLogger(string name) {
            TraceLevel traceLevel = Instance.GetTraceLevel(name);
            return GetLogger(name, traceLevel);
        }

        private TraceLevel GetTraceLevel(string name) {
            var nameKey = name.ToLowerInvariant();
            int maxLengthFound = 0;
            var result = TraceLevel.Info;
            foreach (KeyValuePair<string, TraceLevel> entry in _traceLevelConfig) {
                if (nameKey.StartsWith(entry.Key) && entry.Key.Length > maxLengthFound) {
                    maxLengthFound = entry.Key.Length;
                    result = entry.Value;
                }
            }
            return result;
        }

        public static Logger GetLogger(string name, TraceLevel maxLevel) {
            return new Logger(name, maxLevel);
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
        private readonly string _name;
        private readonly TraceLevel _maxLevel;
        public bool Enabled { get; set; } = true;

        public static int Frame => LoggerFactory.Frame;
        // public static float Delta => LoggerFactory.Delta;

        public Logger(
            string name,
            TraceLevel maxLevel,
            string timeFormat = "yyyy-M-dd HH:mm:ss.fff",
            string traceFormat = "{0} [{1:D4}] {2,5} [{3}] {4}") {
            _name = name;
            _maxLevel = maxLevel;
            TimeFormat = timeFormat;
            TraceFormat = traceFormat;
        }

        public void Error(string message) => Log(TraceLevel.Error, message);
        public void Warning(string message) => Log(TraceLevel.Warning, message);
        public void Info(string message) => Log(TraceLevel.Info, message);
        public void Debug(string message) => Log(TraceLevel.Debug, message);

        public void Error(string message, params object[] args) => Log(TraceLevel.Error, message, args);
        public void Warning(string message, params object[] args) => Log(TraceLevel.Warning, message, args);
        public void Info(string message, params object[] args) => Log(TraceLevel.Info, message, args);
        public void Debug(string message, params object[] args) => Log(TraceLevel.Debug, message, args);

        private void Log(TraceLevel level, string format, params object[] args) =>
            Log(level, string.Format(format, args));

        private void Log(TraceLevel level, string message) {
            if (!Enabled || level > _maxLevel) return;
            var line = string.Format(TraceFormat,
                DateTime.Now.ToString(TimeFormat),
                Frame,
                level.ToString(),
                _name,
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
}