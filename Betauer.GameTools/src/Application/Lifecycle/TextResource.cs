using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Betauer.Application.Lifecycle;

public partial class TextResource(string text) : Resource {

    public string Text { get; } = text;

    public static async Task<TextResource> ReadTextResourceAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        var text = await ReadTextAsync(path, progressCallback, bufferSize, progressIntervalMs);
        return new TextResource(text);
    }

    public static async Task<string> ReadTextAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        if (!FileAccess.FileExists(path)) {
            throw new FileNotFoundException($"Resource not found: {path}");
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var fileLength = file.GetLength();
        var stringBuilder = new StringBuilder((int)fileLength); // Preallocate for efficiency
        ulong bytesRead = 0L;
        var lastProgressUpdate = DateTime.UtcNow;
        var decoder = Encoding.UTF8.GetDecoder();

        while (bytesRead < fileLength) {
            var remainingBytes = fileLength - bytesRead;
            var bytesToRead = (int)Math.Min((ulong)bufferSize, remainingBytes);

            // Read the chunk of bytes
            var byteChunk = file.GetBuffer(bytesToRead);

            // Convert bytes to chars
            var maxCharCount = Encoding.UTF8.GetMaxCharCount(byteChunk.Length);
            var charArray = new char[maxCharCount];
            var charCount = decoder.GetChars(byteChunk, charArray, bytesToRead == (int)remainingBytes);

            // Append the chars to the StringBuilder
            stringBuilder.Append(charArray, 0, charCount);

            bytesRead += (ulong)byteChunk.Length;

            // Update progress if needed
            var now = DateTime.UtcNow;
            if (progressCallback != null && (now - lastProgressUpdate).TotalMilliseconds >= progressIntervalMs) {
                var progress = (float)bytesRead / fileLength;
                await progressCallback(progress);
                lastProgressUpdate = now;
            }
        }

        return stringBuilder.ToString();
    }
}