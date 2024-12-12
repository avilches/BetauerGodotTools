using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core;

public class TextCanvas {
    private readonly List<string> _lines;
    private char _lineSeparator;
    private string? _text = null;

    public int Width { get; private set; } = 0;
    public int Height { get; private set; } = 0;

    public char LineSeparator {
        get => _lineSeparator;
        set {
            _lineSeparator = value;
            _text = null;
        }
    }

    public TextCanvas(char lineSeparator = '\n') : this([""], lineSeparator) {
    }

    public TextCanvas(string initialContent, char lineSeparator = '\n') :
        this(SafeSplit(initialContent, lineSeparator), lineSeparator) {
    }

    private static string[] SafeSplit(string initialContent, char lineSeparator) {
        return string.IsNullOrEmpty(initialContent)
            ? throw new ArgumentException("Initial content cannot be empty")
            : initialContent.Split(lineSeparator);
    }
    
    public TextCanvas(string[] lines, char lineSeparator = '\n') {
        _lines = [..lines];
        LineSeparator = lineSeparator;
        var width = _lines.Max(line => line.Length);
        EnsureSize(width, _lines.Count);
    }

    public void Clear() {
        _lines.Clear();
        Width = 0;
        Height = 1;
    }

    public string GetText() => _text ??= string.Join(LineSeparator, _lines);

    public void Write(int column, int row, string? text) {
        if (string.IsNullOrEmpty(text)) return;
        if (text.Contains(LineSeparator)) {
            var parts = text.Split(LineSeparator);
            for (var i = 0; i < parts.Length; i++) {
                _Write(column, row + i, parts[i]);
            }
        } else {
            _Write(column, row, text);
        }
    }

    public void WriteEnd(int endX, int y, string str) {
        Write(endX - str.Length, y, str);
    }

    private void _Write(int column, int row, string text) {
        if (column < 0) throw new ArgumentException("Column is out of canvas bounds: " + row);
        if (row < 0) throw new ArgumentException("Row is out of canvas bounds: " + row);
        text ??= "";
        EnsureSize(column + text.Length, row + 1);
        var line = _lines[row];
        _lines[row] = line.Substring(0, column) + text + line.Substring(column + text.Length);
        _text = null;
    }

    private void EnsureSize(int width, int height) {
        if (width > Width) {
            for (var i = 0; i < _lines.Count; i++) {
                _lines[i] = _lines[i].PadRight(width);
            }
        }
        Width = width;
        while (height > _lines.Count) {
            _lines.Add(new string(' ', Width));
        }
        Height = _lines.Count;
    }

    public void Fill(int x, int y, int width, int height, char fillChar) {
        EnsureSize(x + width, y + height);

        for (var i = y; i < y + height; i++) {
            var line = _lines[i].ToCharArray();
            for (var j = x; j < x + width; j++) {
                line[j] = fillChar;
            }
            _lines[i] = new string(line);
        }
        _text = null;
    }

    public void WriteCentered(int centerX, int centerY, string text) {
        if (string.IsNullOrEmpty(text)) return;
        var textStartX = centerX - text.Length / 2;
        if (textStartX < 0 || centerY < 0 || centerY >= _lines.Count)
            throw new ArgumentException("Centered text doesn't fit in the canvas.");
        Write(textStartX, centerY, text);
    }

    public override string ToString() => GetText();
}