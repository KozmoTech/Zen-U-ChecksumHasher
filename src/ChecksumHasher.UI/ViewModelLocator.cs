namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// An utility class to locate the global (singleton) instances of ViewModels.
/// </summary>
internal static class ViewModelLocator
{
    /// <summary>
    /// Get a singleton instance of <see cref="ChecksumHasher.FileHasherViewModel"/> which can be used to calculate checksums of a file.
    /// </summary>
    public static FileHasherViewModel FileHasherViewModel => fileHasher.Value;

    /// <summary>
    /// Get a singleton instance of <see cref="ChecksumHasher.AppSettingsViewModel"/> which represents the application runtime settings.
    /// </summary>
    public static AppSettingsViewModel SettingsViewModel => settings.Value;

    private static readonly Lazy<AppSettingsViewModel> settings = new();
    private static readonly Lazy<FileHasherViewModel> fileHasher = new();
}
