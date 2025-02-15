using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Betauer.Application.Lifecycle;

public partial class BinaryResource(byte[] data) : Resource {
    public byte[] Data { get; } = data;


    public static async Task<BinaryResource> ReadBinaryResourceAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        var data = await ReadAsync(path, progressCallback, bufferSize, progressIntervalMs);
        return new BinaryResource(data);
    }

    public static async Task<byte[]> ReadAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        // Check if the resource exists
        if (!FileAccess.FileExists(path)) {
            throw new FileNotFoundException($"Resource not found: {path}");
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var fileLength = file.GetLength();
        var result = new byte[fileLength];
        ulong bytesRead = 0L;
        var lastProgressUpdate = DateTime.UtcNow;

        while (bytesRead < fileLength) {
            // Calculate how many bytes to read in this iteration
            var remainingBytes = fileLength - bytesRead;
            var bytesToRead = (int)Math.Min((ulong)bufferSize, remainingBytes);

            // Read the chunk
            var chunk = file.GetBuffer(bytesToRead);

            // Copy to the result array
            Array.Copy(chunk, 0, result, (long)bytesRead, chunk.Length);
            bytesRead += (ulong)chunk.Length;

            // Check if we should notify progress
            var now = DateTime.UtcNow;
            if (progressCallback != null && (now - lastProgressUpdate).TotalMilliseconds >= progressIntervalMs) {
                var progress = (float)bytesRead / fileLength;
                await progressCallback(progress);
                lastProgressUpdate = now;
            }
        }

        return result;
    }
}