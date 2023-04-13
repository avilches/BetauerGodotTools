using System;
using System.Collections.Generic;
using System.IO;
using Betauer.Core;

namespace Betauer.Tools.Logging;

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
        return GetLogger(type.GetTypeName());
    }

    public static Logger GetLogger(string type) {
        var key = type.ToLower();
        if (!Loggers.TryGetValue(key, out var logger)) {
            logger = new Logger(type);
            Loggers.Add(key, logger);
        }
        return logger;
    }

    public static void SetTraceLevel<T>(TraceLevel traceLevel) {
        GetLogger(typeof(T)).SetTraceLevel(traceLevel);
    }
		
    public static void SetTraceLevel(Type type, TraceLevel traceLevel) {
        GetLogger(type).SetTraceLevel(traceLevel);
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