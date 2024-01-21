namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

/// <summary>
/// A platform-independent file details object.
/// </summary>
public interface IFileInfo
{
    Task LoadPropertiesAsync();
    Task LoadIconAsync();
    Task<Stream> OpenSequentialReadAsync();

    string FullPath { get; }
    ulong? Length { get; }
    string? Type { get; }
    IPicture? Icon { get; }
    DateTimeOffset? CreatedAt { get; }
    DateTimeOffset? ModifiedAt { get; }
}
