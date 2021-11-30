using KozmoTech.ZenUtility.System.IO;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal interface IContentProvider
{
    /// <summary>
    /// Create a new instance of <see cref="StreamReaderWithProgress"/>, the caller is responsible for disposing it.
    /// </summary>
    /// <returns>A <see cref="StreamReaderWithProgress"/> instance which can be used to read content.</returns>
    Task<StreamReaderWithProgress> CreateContentReaderAsync();

    ulong TotalLength { get; }
}
