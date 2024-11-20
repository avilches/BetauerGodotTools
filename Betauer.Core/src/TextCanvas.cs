using System;
using System.Linq;

namespace Betauer.Core;

public class TextCanvas {
    private readonly string[] _lines;
    private char _lineSeparator;
    private string? _text = null;

    public int Width { get; }
    public int Height { get; }

    public char LineSeparator {
        get => _lineSeparator;
        set {
            _lineSeparator = value;
            _text = null;
        }
    }

    public TextCanvas(int width, int height, char lineSeparator = '\n') :
        this(Enumerable.Repeat(new string(' ', width), height).ToArray(), lineSeparator) {
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
        if (lines.Length == 0) throw new ArgumentException("Initial content cannot be empty");
        _lines = lines;
        Width = _lines[0].Length;
        Height = _lines.Length;
        LineSeparator = lineSeparator;

        if (_lines.Any(line => line.Length != Width)) throw new ArgumentException("All lines must be the same length");
    }

    public string GetText() => _text ??= string.Join(LineSeparator, _lines);

    public void Write(int column, int row, string text) {
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
        if (string.IsNullOrEmpty(text)) return;
        if (row < 0 || row >= _lines.Length) throw new ArgumentOutOfRangeException(nameof(row), $"Row is out of canvas bounds: {row}");
        if (column < 0 || column >= Width) throw new ArgumentOutOfRangeException(nameof(column), $"Column is out of canvas bounds: {column}");
        if (column + text.Length > Width) throw new ArgumentException($"Text is too wide: {text.Length}. From the position {column}, the space is only {Width - column} chars long.");
        var line = _lines[row];
        _lines[row] = line.Substring(0, column) + text + line.Substring(column + text.Length);
        _text = null;
    }

    public void Fill(int x, int y, int width, int height, char fillChar) {
        if (x < 0 || x >= Width || y < 0 || y >= _lines.Length)
            throw new ArgumentException("Region starts outside of the canvas");
        if (x + width > Width || y + height > _lines.Length)
            throw new ArgumentException("Region doesn't fit in the canvas.");

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
        if (textStartX < 0 || centerY < 0 || centerY >= _lines.Length)
            throw new ArgumentException("Centered text doesn't fit in the canvas.");
        Write(textStartX, centerY, text);
    }

    public override string ToString() => GetText();
}