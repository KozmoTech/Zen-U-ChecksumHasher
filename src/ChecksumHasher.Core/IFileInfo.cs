namespace KozmoTech.ZenUtility.ChecksumHasher;

/// <summary>
/// A platform-independent file details object.
/// </summary>
public interface IFileInfo
{
    Task LoadPropertiesAsync();
    Task<Stream> OpenSequentialReadAsync();

    string FullPath { get; }
    ulong Length { get; }
}
