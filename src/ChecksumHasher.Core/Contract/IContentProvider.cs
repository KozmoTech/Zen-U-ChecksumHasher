namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

internal interface IContentProvider
{
    /// <summary>
    /// Create a new instance of <see cref="Stream"/>, the caller is responsible for disposing it.
    /// </summary>
    /// <returns>A <see cref="Stream"/> instance which can be used to read content.</returns>
    Task<Stream> CreateContentReaderAsync();

    ulong? TotalLength { get; }
}
