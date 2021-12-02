using Windows.Storage;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal sealed class AppSettingsStorage : ISettingsStorage
{
    private AppSettingsStorage()
    {
    }

    public static AppSettingsStorage Default => instance.Value;

    public ISettingsStorageContainer RootContainer { get; } = new WindowsAppDataContainer(ApplicationData.Current.LocalSettings);

    private static readonly Lazy<AppSettingsStorage> instance = new(() => new());
}

internal sealed class WindowsAppDataContainer : ISettingsStorageContainer
{
    public WindowsAppDataContainer(ApplicationDataContainer container) => this.container = container ?? throw new ArgumentNullException(nameof(container));

    public ISettingsStorageContainer EnsureSubContainer(string key)
    {
        var underlyingContainer = container.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        return new WindowsAppDataContainer(underlyingContainer);
    }

    public void DeleteValue(string key) => container.Values.Remove(key);

    public string? ReadStringValue(string key) => (string?)container.Values[key];

    public void SaveValue(string key, string value) => container.Values[key] = value;

    private readonly ApplicationDataContainer container;
}
