using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.Reflection;
using Godot;
using File = System.IO.File;

namespace Betauer {
    public enum TraceLevel {
        Off = 0,
        Fatal = 1,
        Error = 2,
        Warning = 3,
        Info = 4,
        Debug = 5,
        All = 5,
    }
    
    public enum ConsoleOutput {
        GodotPrint,
        ConsoleWriteLine,
        Off
    }

    public class LoggerFactory {
        internal static readonly string[] TraceLevelAsString = new[] {
            string.Empty,
            "[Fatal]",
            "[Error]",
            "[Warn ]",
            "[Info ]",
            "[Debug]",
        };


        public static LoggerFactory Instance { get; } = new LoggerFactory();
        internal static ITextWriter[] Writers => Instance._writers;

        public Dictionary<string, Logger> Loggers { get; } = new Dictionary<string, Logger>();
        private ITextWriter[] _writers = { };
        internal bool IncludeTimestamp = true;

        public TraceLevel DefaultTraceLevelConfig = TraceLevel.Error;
        public ConsoleOutput ConsoleOutput = ConsoleOutput.GodotPrint;

        private LoggerFactory() {
        }

        public static void Reset() {
            Instance.DefaultTraceLevelConfig = TraceLevel.Error;
            Instance.ConsoleOutput = ConsoleOutput.GodotPrint;
            Instance.Loggers.Clear();
            Instance._writers = Array.Empty<ITextWriter>();
        }

        public static LoggerFactory SetConsoleOutput(ConsoleOutput consoleOutput) {
            Instance.ConsoleOutput = consoleOutput;
            return Instance;
        }

        public static LoggerFactory AddFileWriter(string logPath) {
            return AddTextWriter(File.CreateText(logPath));
        }

        public static LoggerFactory AddTextWriter(TextWriter textWriter) {
            return Instance.AddTextWriter(new TextWriterWrapper(textWriter));
        }

        public static LoggerFactory SetTraceLevel(Type type, TraceLevel traceLevel) {
            return SetTraceLevel(type.GetNameWithoutGenerics(), traceLevel);
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
            Instance.Loggers.Values.ForEach(l => {
                if (l.HasMaxTraceLevel) l.SetTraceLevel(traceLevel);
            });
            return Instance;
        }

        public static Logger GetLogger<T>() {
            return GetLogger(typeof(T));
        }

        public static Logger GetLogger(Type type) {
            return GetLogger(type.GetNameWithoutGenerics());
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
                ? new [] { StringTools.FastFormatDateTime(DateTime.Now), " ", levelAsString, " ", Engine.GetIdleFrames().ToString(), " ", _title, " ", message }
                : new [] { levelAsString, _title, message };
            var logLine = StringTools.JoinString(data);

            if (LoggerFactory.Writers.Length > 0) {
                foreach (ITextWriter writer in LoggerFactory.Writers) {
                    writer.WriteLine(logLine);
                    writer.Flush();
                }
            }

            if (LoggerFactory.Instance.ConsoleOutput == ConsoleOutput.GodotPrint) GD.Print(logLine);
            else if (LoggerFactory.Instance.ConsoleOutput == ConsoleOutput.ConsoleWriteLine) Console.WriteLine(logLine);
        }
    }

    public interface ITextWriter {
        void WriteLine(string line);
        void Flush();
        void EnableAutoFlush();
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