namespace KozmoTech.ZenUtility.System.IO;

/// <summary>
/// A forward-only and read-only stream implementation, which will also reports the bytes that have already been read.
/// </summary>
internal sealed class StreamReaderWithProgress : Stream
{
    public StreamReaderWithProgress(Stream innerStream) => this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            innerStream?.Dispose();
        }
        base.Dispose(disposing);
    }

    public override bool CanWrite => false;
    public override bool CanSeek => false;
    public override void Flush() => throw new NotSupportedException("flush is not supported");
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("write is not supported");
    public override void SetLength(long value) => throw new NotSupportedException("length cannot be set");
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("this is a forward-only stream");
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("Read is not supported, please use ReadAsync");

    public override long Length => innerStream.Length;

    /// <summary>
    /// Gets a long representing the number of bytes already read by the user.
    /// </summary>
    public long LengthRead { get; private set; } = 0;

    public override bool CanRead => innerStream.CanRead;
    public override long Position { get => innerStream.Position; set => throw new NotSupportedException("this is a forward-only stream"); }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellation) =>
        ReadAsync(buffer.AsMemory(offset, count), cancellation).AsTask();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellation = default)
    {
        var readSize = await innerStream.ReadAsync(buffer, cancellation);
        LengthRead += readSize;
        return readSize;
    }

    private readonly Stream innerStream;
}
