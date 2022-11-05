using System;
using System.Collections.Generic;
using System.IO;
using File = System.IO.File;

namespace Betauer.Tools.Logging {
    public enum TraceLevel {
        Off = 0,
        Fatal = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
        Debug = 5,
        All = 5,
    }
    
    public class LoggerFactory {
        internal static readonly string[] TraceLevelAsString = {
            string.Empty,
            "[Fatal]",
            "[Error]",
            "[Warn ]",
            "[Info ]",
            "[Debug]",
        };


        public static LoggerFactory Instance { get; } = new();
        internal static ITextWriter[] Writers => Instance._writers;
        internal static ITextWriter? DefaultWriter => Instance._defaultWriter;

        public Dictionary<string, Logger> Loggers { get; } = new();
        private ITextWriter[] _writers = { };
        private ITextWriter? _defaultWriter = new SimpleWriterWrapper(Console.WriteLine);
        internal bool IncludeTimestamp = true;

        public TraceLevel DefaultTraceLevelConfig = TraceLevel.Error;

        private LoggerFactory() {
        }

        public static void Reset() {
            Instance.DefaultTraceLevelConfig = TraceLevel.Error;
            Instance.Loggers.Clear();
            Instance._writers = Array.Empty<ITextWriter>();
        }

        public static LoggerFactory SetDefaultWriter(ITextWriter defaultWriter) {
            Instance._defaultWriter = defaultWriter;
            return Instance;
        }

        public static LoggerFactory AddWriter(ITextWriter writer) {
            return Instance.AddTextWriter(writer);
        }

        public static LoggerFactory AddFileWriter(string logPath) {
            return AddTextWriter(File.CreateText(logPath));
        }

        public static LoggerFactory AddTextWriter(TextWriter textWriter) {
            return AddWriter(new TextWriterWrapper(textWriter));
        }

        public static LoggerFactory AddWriter(Action<string> writer) {
            return AddWriter(new SimpleWriterWrapper(writer));
        }

        public static LoggerFactory SetTraceLevel(Type type, TraceLevel traceLevel) {
            return SetTraceLevel(GetNameWithoutGenerics(type), traceLevel);
        }

        public static LoggerFactory SetTraceLevel(string type, TraceLevel traceLevel) {
            GetLogger(type).SetTraceLevel(traceLevel);
            return Instance;
        }

        public static LoggerFactory SetIncludeTimestamp(bool includeTimestamp) {
            Instance.IncludeTimestamp = includeTimestamp;
            return Instance;
        }

        public static LoggerFactory SetDefaultTraceLevel(TraceLevel traceLevel) {
            Instance.DefaultTraceLevelConfig = traceLevel;
            return Instance;
        }

        public static LoggerFactory OverrideTraceLevel(TraceLevel traceLevel) {
            SetDefaultTraceLevel(traceLevel);
            foreach (var l in Instance.Loggers.Values)  {
                if (l.HasMaxTraceLevel) l.SetTraceLevel(traceLevel);
            };
            return Instance;
        }

        public static Logger GetLogger<T>() {
            return GetLogger(typeof(T));
        }

        public static Logger GetLogger(Type type) {
            return GetLogger(GetNameWithoutGenerics(type));
        }

        public static Logger GetLogger(string type) {
            var key = type.ToLower();
            if (!Instance.Loggers.TryGetValue(key, out var logger)) {
                logger = new Logger(type);
                Instance.Loggers.Add(key, logger);
            }
            return logger;
        }

        public static void EnableAutoFlush() {
            foreach (ITextWriter writer in Instance._writers) writer.EnableAutoFlush();
        }

        private LoggerFactory AddTextWriter(ITextWriter textWriter) {
            ITextWriter[] writers = new ITextWriter[_writers.Length + 1];
            _writers.CopyTo(writers, 0);
            writers[_writers.Length] = textWriter;
            _writers = writers;
            return this;
        }

        private static string GetNameWithoutGenerics(Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name[..index];
        }

    }

    public class Logger {
        public TraceLevel MaxTraceLevel => HasMaxTraceLevel ? _loggerMaxTraceLevel : LoggerFactory.Instance.DefaultTraceLevelConfig;
        public readonly string Type;
        public bool HasMaxTraceLevel { get; private set; } = false;
        public bool Enabled { get; set; } = true;

        private TraceLevel _loggerMaxTraceLevel;
        private readonly string _title;

        internal Logger(string type) {
            Type = type;
            _title = $"[{type}]";
        }

        public void Fatal(Exception e) => Log(TraceLevel.Fatal, e);
        public void Error(Exception e) => Log(TraceLevel.Error, e);
        public void Warning(Exception e) => Log(TraceLevel.Warning, e);
        public void Info(Exception e) => Log(TraceLevel.Info, e);
        public void Debug(Exception e) => Log(TraceLevel.Debug, e);

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

        public bool IsEnabled() {
            return Enabled;
        }

        public Logger SetTraceLevel(TraceLevel traceLevel) {
            _loggerMaxTraceLevel = traceLevel;
            HasMaxTraceLevel = true;
            return this;
        }

        public bool IsEnabled(TraceLevel level) {
            return Enabled && level <= MaxTraceLevel;
        }

        private void Log(TraceLevel level, Exception e) {
            Log(level, e.ToString());
        }

        private void Log(TraceLevel level, string message) {
            if (!IsEnabled(level)) return;
            
            var levelAsString = LoggerFactory.TraceLevelAsString[(int)level];
            var data = LoggerFactory.Instance.IncludeTimestamp
                ? new [] { FastDateFormatter.FastFormatDateTime(DateTime.Now), " ", levelAsString, " ", " ", _title, " ", message }
                : new [] { levelAsString, _title, message };
            var logLine = FastDateFormatter.JoinString(data);

            var size = LoggerFactory.Writers.Length;
            if (size > 0) {
                Span<ITextWriter> span = LoggerFactory.Writers;
                for (var i = 0; i < size; i++) {
                    var writer = span[i];
                    writer.WriteLine(logLine);
                    writer.Flush();
                }
            } else if (LoggerFactory.DefaultWriter != null) {
                LoggerFactory.DefaultWriter.WriteLine(logLine);
                LoggerFactory.DefaultWriter.Flush();
            }
        }
    }

    public interface ITextWriter {
        void WriteLine(string line);
        void Flush();
        void EnableAutoFlush();
    }

    public class SimpleWriterWrapper : ITextWriter {
        private readonly Action<string> _writer;

        public SimpleWriterWrapper(Action<string> writer) {
            _writer = writer;
        }

        public void WriteLine(string line) {
            _writer(line);
        }

        public void Flush() {
        }

        public void EnableAutoFlush() {
        }
    }

    public class TextWriterWrapper : ITextWriter {
        private readonly TextWriter _writer;
        private bool _autoFlush = false;

        public TextWriterWrapper(TextWriter writer) {
            _writer = writer;
        }

        public void WriteLine(string line) {
            _writer.WriteLine(line);
            if (_autoFlush) _writer.Flush();
        }

        public void Flush() {
            _writer.Flush();
        }

        public void EnableAutoFlush() {
            if (_autoFlush) return;
            _autoFlush = true;
            _writer.Flush();
        }
    }
}