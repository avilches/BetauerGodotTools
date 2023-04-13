using System;
using Cysharp.Text;
using Godot;

namespace Betauer.Tools.Logging; 

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

	public void Fatal(string message) => Log(TraceLevel.Fatal, message, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Error(string message) => Log(TraceLevel.Error, message, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Warning(string message) => Log(TraceLevel.Warning, message, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Info(string message) => Log(TraceLevel.Info, message, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug(string message) => Log(TraceLevel.Debug, message, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1>(string message, T1 p1) => Log(TraceLevel.Fatal, message, 1, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Error<T1>(string message, T1 p1) => Log(TraceLevel.Error, message, 1, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Warning<T1>(string message, T1 p1) => Log(TraceLevel.Warning, message, 1, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Info<T1>(string message, T1 p1) => Log(TraceLevel.Info, message, 1, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1>(string message, T1 p1) => Log(TraceLevel.Debug, message, 1, p1, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Fatal, message, 2, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Error<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Error, message, 2, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Warning<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Warning, message, 2, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Info<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Info, message, 2, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2>(string message, T1 p1, T2 p2) => Log(TraceLevel.Debug, message, 2, p1, p2, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Fatal, message, 3, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Error<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Error, message, 3, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Warning<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Warning, message, 3, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Info<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Info, message, 3, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3>(string message, T1 p1, T2 p2, T3 p3) => Log(TraceLevel.Debug, message, 3, p1, p2, p3, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Fatal, message, 4, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Error<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Error, message, 4, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Warning<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Warning, message, 4, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
	public void Info<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Info, message, 4, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3, T4>(string message, T1 p1, T2 p2, T3 p3, T4 p4) => Log(TraceLevel.Debug, message, 4, p1, p2, p3, p4, string.Empty, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Fatal, message, 5, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
	public void Error<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Error, message, 5, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
	public void Warning<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Warning, message, 5, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
	public void Info<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Info, message, 5, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3, T4, T5>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => Log(TraceLevel.Debug, message, 5, p1, p2, p3, p4, p5, string.Empty, string.Empty, string.Empty);

	public void Fatal<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Fatal, message, 6, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
	public void Error<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Error, message, 6, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
	public void Warning<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Warning, message, 6, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
	public void Info<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Info, message, 6, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3, T4, T5, T6>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => Log(TraceLevel.Debug, message, 6, p1, p2, p3, p4, p5, p6, string.Empty, string.Empty);

	public void Fatal<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Fatal, message, 7, p1, p2, p3, p4, p5, p6, p7, string.Empty);
	public void Error<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Error, message, 7, p1, p2, p3, p4, p5, p6, p7, string.Empty);
	public void Warning<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Warning, message, 7, p1, p2, p3, p4, p5, p6, p7, string.Empty);
	public void Info<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Info, message, 7, p1, p2, p3, p4, p5, p6, p7, string.Empty);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3, T4, T5, T6, T7>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => Log(TraceLevel.Debug, message, 7, p1, p2, p3, p4, p5, p6, p7, string.Empty);

	public void Fatal<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Fatal, message, 8, p1, p2, p3, p4, p5, p6, p7, p8);
	public void Error<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Error, message, 8, p1, p2, p3, p4, p5, p6, p7, p8);
	public void Warning<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Warning, message, 8, p1, p2, p3, p4, p5, p6, p7, p8);
	public void Info<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Info, message, 8, p1, p2, p3, p4, p5, p6, p7, p8);
	[System.Diagnostics.Conditional("DEBUG")]
	public void Debug<T1, T2, T3, T4, T5, T6, T7, T8>(string message, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => Log(TraceLevel.Debug, message, 8, p1, p2, p3, p4, p5, p6, p7, p8);

	private void Log<T1, T2, T3, T4, T5, T6, T7, T8>(TraceLevel level, string message, int args, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) {
		if (!IsEnabled(level)) return;
		var line = CreateLogLine(level, message, args, p1, p2, p3, p4, p5, p6, p7, p8);
		LoggerFactory.Write(level, line);
	}

	private string CreateLogLine<T1, T2, T3, T4, T5, T6, T7, T8>(TraceLevel level, string message, int args, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) {
		var traceLevelAsString = LoggerFactory.TraceLevelAsString[(int)level];
		using var sb = ZString.CreateStringBuilder();
		if (LoggerFactory.IncludeTimestamp) {
			sb.Append(DateTime.Now, _dateFormat ?? LoggerFactory.DefaultDateFormat);
			sb.Append(' ');
		}
		sb.Append(traceLevelAsString);
		sb.Append(" Idle #");
		sb.Append(Engine.GetProcessFrames().ToString());
		sb.Append(" | Physics #");
		sb.Append(Engine.GetPhysicsFrames().ToString());
		sb.Append(' ');
		sb.Append(_name);
		sb.Append(' ');
		if (args == 0) sb.Append(message);
		else if (args == 1) sb.AppendFormat(message, p1);
		else if (args == 2) sb.AppendFormat(message, p1, p2);
		else if (args == 3) sb.AppendFormat(message, p1, p2, p3);
		else if (args == 4) sb.AppendFormat(message, p1, p2, p3, p4);
		else if (args == 5) sb.AppendFormat(message, p1, p2, p3, p4, p5);
		else if (args == 6) sb.AppendFormat(message, p1, p2, p3, p4, p5, p6);
		else if (args == 7) sb.AppendFormat(message, p1, p2, p3, p4, p5, p6, p7);
		else if (args == 8) sb.AppendFormat(message, p1, p2, p3, p4, p5, p6, p7, p8);
		return sb.ToString();
	}
}