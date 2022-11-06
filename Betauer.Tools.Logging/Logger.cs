using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Text;
using Godot;
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
    
    public static class LoggerFactory {
        internal static readonly string[] TraceLevelAsString = {
            string.Empty,
            "[Fatal]",
            "[Error]",
            "[Warn ]",
            "[Info ]",
            "[Debug]",
        };

        public static ITextWriter[] Writers { get; private set; } = { };
        public static Dictionary<string, Logger> Loggers { get; } = new();
        
        public static TraceLevel DefaultTraceLevelConfig  { get; private set; } = TraceLevel.Error;
        public static string DefaultDateFormat  { get; private set; } = "yyyy-mm-dd hh:mm:ss.mmm";
        public static bool IncludeTimestamp  { get; private set; } = true;
        public static bool AutoFlush { get; private set; } = false;
        public static ITextWriter? DefaultWriter  { get; private set; } = GDPrintWriter.Instance;

        public static void Reset() {
            Writers = Array.Empty<ITextWriter>();
            Loggers.Clear();

            DefaultTraceLevelConfig = TraceLevel.Error;
            DefaultDateFormat = "yyyy-mm-dd hh:mm:ss.mmm";
            IncludeTimestamp = true;
            AutoFlush = false;
            DefaultWriter = GDPrintWriter.Instance; 
        }

        /// <summary>
        /// Loggers without a tracelevel defined will use this default trace (loggers are created without tracelevel by default)
        /// </summary>
        /// <param name="traceLevel"></param>
        public static void SetDefaultTraceLevel(TraceLevel traceLevel) {
            DefaultTraceLevelConfig = traceLevel;
        }

        /// <summary>
        /// Loggers without a dateformet defined will use this default dateformat (loggers are created without dateformat by default).
        /// </summary>
        /// <param name="defaultDateFormat"></param>
        public static void SetDefaultDateFormat(string defaultDateFormat) {
            DefaultDateFormat = defaultDateFormat;
        }

        /// <summary>
        /// true = logs include the timestamp, like this:
        /// 2022-56-06 09:56:12.56 [Info ] [EmailService] This is a message
        /// 
        /// false = logs will not include the timestamp, like this
        /// [Info ] [EmailService] This is a message
        /// </summary>
        /// <param name="includeTimestamp"></param>
        public static void SetIncludeTimestamp(bool includeTimestamp) {
            IncludeTimestamp = includeTimestamp;
        }

        /// <summary>
        /// The default writer will be used when there is no writers defined. As soon as one writer is defined, the
        /// default will not be used.
        /// </summary>
        /// <param name="defaultWriter"></param>
        public static void SetDefaultWriter(ITextWriter defaultWriter) {
            DefaultWriter = defaultWriter;
        }

        /// <summary>
        /// Flush all writers right now. After this method, all writers will be flush in every message logged.
        /// </summary>
        /// <param name="autoFlush"></param>
        public static void SetAutoFlush(bool autoFlush) {
            AutoFlush = autoFlush;
            foreach (var l in Writers) l.Flush();
        }

        /// <summary>
        /// Override the tracelevel for all loggers and force them to use this tracelevel. It also make the specified
        /// tracelevel as default tracelevel too, so next logger created after this method call will use the new
        /// tracelevel.
        ///
        /// </summary>
        /// <param name="traceLevel"></param>
        public static void OverrideTraceLevel(TraceLevel traceLevel) {
            SetDefaultTraceLevel(traceLevel);
            foreach (var l in Loggers.Values)  {
                if (l.HasMaxTraceLevel) l.SetTraceLevel(traceLevel);
            }
        }
        
        public static void AddWriter(ITextWriter writer) {
            AddTextWriter(writer);
        }

        public static void AddFileWriter(string logPath) { 
            AddTextWriter(File.CreateText(logPath));
        }

        public static void AddTextWriter(TextWriter textWriter) {
            AddWriter(new TextWriterWrapper(textWriter));
        }

        public static void AddWriter(Action<string> writer) {
            AddWriter(new SimpleWriterWrapper((_, message) => writer(message)));
        }

        public static void AddWriter(Action<TraceLevel, string> writer) {
            AddWriter(new SimpleWriterWrapper(writer));
        }

        public static Logger GetLogger<T>() {
            return GetLogger(typeof(T));
        }

        public static Logger GetLogger(Type type) {
            return GetLogger(GetNameWithoutGenerics(type));
        }

        public static Logger GetLogger(string type) {
            var key = type.ToLower();
            if (!Loggers.TryGetValue(key, out var logger)) {
                logger = new Logger(type);
                Loggers.Add(key, logger);
            }
            return logger;
        }

        public static void SetTraceLevel(Type type, TraceLevel traceLevel) {
            SetTraceLevel(GetNameWithoutGenerics(type), traceLevel);
        }

        public static void SetTraceLevel(string type, TraceLevel traceLevel) {
            GetLogger(type).SetTraceLevel(traceLevel);
        }

        private static void AddTextWriter(ITextWriter textWriter) {
            ITextWriter[] writers = new ITextWriter[Writers.Length + 1];
            Writers.CopyTo(writers, 0);
            writers[Writers.Length] = textWriter;
            Writers = writers;
        }

        private static string GetNameWithoutGenerics(Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name[..index];
        }

        public static void Write(TraceLevel level, string logLine) {
            var size = Writers.Length;
            if (size > 0) {
                Span<ITextWriter> span = Writers;
                for (var i = 0; i < size; i++) {
                    var writer = span[i];
                    writer.WriteLine(level, logLine);
                    if (AutoFlush) writer.Flush();
                }
            } else if (DefaultWriter != null) {
                DefaultWriter.WriteLine(level, logLine);
                if (AutoFlush) DefaultWriter.Flush();
            }
        }
    }

    public class Logger {
        public TraceLevel MaxTraceLevel => HasMaxTraceLevel ? _loggerMaxTraceLevel : LoggerFactory.DefaultTraceLevelConfig;
        public readonly string Name;
        public bool HasMaxTraceLevel { get; private set; } = false;
        public bool Enabled { get; set; } = true;

        private string? _dateFormat;
        private TraceLevel _loggerMaxTraceLevel;
        private readonly string _name;

        internal Logger(string name) {
            Name = name;
            _name = $"[{name}]";
        }

        public void Enable(bool enable) => Enabled = enable;
        
        public bool IsEnabled(TraceLevel level) => Enabled && level <= MaxTraceLevel;

        public void UseDefaultTraceLevel() {
            HasMaxTraceLevel = false;
        }

        public void SetTraceLevel(TraceLevel traceLevel) {
            _loggerMaxTraceLevel = traceLevel;
            HasMaxTraceLevel = true;
        }

        public void SetDateFormat(string dateFormat) {
            _dateFormat = dateFormat;
        }

        public void Fatal(string message) => Log(TraceLevel.Fatal, message, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Error(string message) => Log(TraceLevel.Error, message, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Warning(string message) => Log(TraceLevel.Warning, message, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Info(string message) => Log(TraceLevel.Info, message, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug(string message) => Log(TraceLevel.Debug, message, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1>(string message, T1 p1) => Log(TraceLevel.Fatal, message, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Error<T1>(string message, T1 p1) => Log(TraceLevel.Error, message, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Warning<T1>(string message, T1 p1) => Log(TraceLevel.Warning, message, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Info<T1>(string message, T1 p1) => Log(TraceLevel.Info, message, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1>(string message, T1 p1) => Log(TraceLevel.Debug, message, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Fatal, message, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Error<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Error, message, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Warning<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Warning, message, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Info<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Info, message, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Debug, message, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Fatal, message, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Error<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Error, message, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Warning<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Warning, message, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Info<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Info, message, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Debug, message, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Fatal, message, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Error<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Error, message, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Warning<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Warning, message, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
        public void Info<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Info, message, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Debug, message, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Fatal, message, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
        public void Error<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Error, message, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
        public void Warning<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Warning, message, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
        public void Info<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Info, message, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Debug, message, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);

        public void Fatal<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Fatal, message, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
        public void Error<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Error, message, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
        public void Warning<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Warning, message, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
        public void Info<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Info, message, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Debug, message, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);

        public void Fatal<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Fatal, message, p1, p2, p3, p4, p5, p6, p7, string.Empty);
        public void Error<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Error, message, p1, p2, p3, p4, p5, p6, p7, string.Empty);
        public void Warning<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Warning, message, p1, p2, p3, p4, p5, p6, p7, string.Empty);
        public void Info<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Info, message, p1, p2, p3, p4, p5, p6, p7, string.Empty);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Debug, message, p1, p2, p3, p4, p5, p6, p7, string.Empty);

        public void Fatal<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Fatal, message, p1, p2, p3, p4, p5, p6, p7, p8);
        public void Error<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Error, message, p1, p2, p3, p4, p5, p6, p7, p8);
        public void Warning<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Warning, message, p1, p2, p3, p4, p5, p6, p7, p8);
        public void Info<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Info, message, p1, p2, p3, p4, p5, p6, p7, p8);
        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Debug, message, p1, p2, p3, p4, p5, p6, p7, p8);

        private void Log<T1, T2, T3, T4, T5, T6, T7, T8>(TraceLevel level, string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) {
            if (!IsEnabled(level)) return;
            var line = CreateLogLine(level, message, p1, p2, p3, p4, p5, p6, p7, p8);
            LoggerFactory.Write(level, line);
        }

        private string CreateLogLine<T1, T2, T3, T4, T5, T6, T7, T8>(TraceLevel level, string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) {
            var traceLevelAsString = LoggerFactory.TraceLevelAsString[(int)level];
            using var sb = ZString.CreateStringBuilder();
            if (LoggerFactory.IncludeTimestamp) {
                sb.Append(DateTime.Now, _dateFormat ?? LoggerFactory.DefaultDateFormat);
                sb.Append(' ');
            }
            sb.Append(traceLevelAsString);
            sb.Append(' ');
            sb.Append(_name);
            sb.Append(' ');
            sb.AppendFormat(message, p1, p2, p3, p4, p5, p6, p7, p8);
            return sb.ToString();
        }
    }

    public interface ITextWriter {
        void WriteLine(TraceLevel level, string line);
        void Flush();
    }

    public class SimpleWriterWrapper : ITextWriter {
        private readonly Action<TraceLevel, string> _writer;

        public SimpleWriterWrapper(Action<TraceLevel, string> writer) {
            _writer = writer;
        }

        public void WriteLine(TraceLevel level, string line) {
            _writer?.Invoke(level, line);
        }

        public void Flush() {
        }
    }

    public class ConsoleWriter : ITextWriter {
        public static readonly ConsoleWriter Instance = new();

        public void WriteLine(TraceLevel level, string line) {
            Console.WriteLine(line);
        }

        public void Flush() {
        }
    }

    public class ColoredConsoleWriter : ITextWriter {
        public static readonly ColoredConsoleWriter Instance = new();

        public void WriteLine(TraceLevel level, string line) {
            switch (level) {
                case TraceLevel.Fatal:
                case TraceLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(line);
                    Console.ResetColor();
                    break;
                case TraceLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(line);
                    Console.ResetColor();
                    break;
                default:
                    Console.WriteLine(line);
                    break;
            }
        }

        public void Flush() {
        }
    }

    public class GDPrintWriter : ITextWriter {
        public static readonly GDPrintWriter Instance = new();

        public void WriteLine(TraceLevel level, string line) {
            switch (level) {
                case TraceLevel.Fatal:
                case TraceLevel.Error:
                    GD.PushError(line);
                    break;
                case TraceLevel.Warning:
                    GD.PushWarning(line);
                    break;
                default:
                    GD.Print(line);
                    break;
            }
        }

        public void Flush() {
        }
    }

    public class TextWriterWrapper : ITextWriter {
        private readonly TextWriter _writer;

        public TextWriterWrapper(TextWriter writer) => _writer = writer;

        public void WriteLine(TraceLevel level, string line) => _writer.WriteLine(line);

        public void Flush() => _writer.Flush();
    }
}