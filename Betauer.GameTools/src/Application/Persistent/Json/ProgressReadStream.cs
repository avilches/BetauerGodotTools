using System;
using System.IO;

namespace Betauer.Application.Persistent.Json;

public class ProgressReadStream : Stream {
    private readonly Stream _baseStream;
    private readonly Action<long> _onRead;
    private long _read = 0;

    public ProgressReadStream(Stream baseStream, Action<long> onRead) {
        _baseStream = baseStream;
        _onRead = onRead;
    }

    public override void Flush() {
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count) {
        var read = _baseStream.Read(buffer, offset, count);
        _read += read;
        _onRead.Invoke(_read);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin) {
        return _baseStream.Seek(offset, origin);
    }

    public override void SetLength(long value) {
        _baseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count) {
        _baseStream.Write(buffer, offset, count);
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length;
    public override long Position {
        get => _baseStream.Position;
        set => _baseStream.Position = value;
    }
}