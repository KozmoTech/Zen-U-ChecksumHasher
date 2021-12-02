namespace KozmoTech.ZenUtility.ChecksumHasher;

public interface ISettingsStorage
{
    ISettingsStorageContainer RootContainer { get; }
}

public interface ISettingsStorageContainer
{
    ISettingsStorageContainer EnsureSubContainer(string key);

    void DeleteValue(string key);

    string? ReadStringValue(string key);
    void SaveValue(string key, string value);

    T? ReadEnumValue<T>(string key)
        where T : struct, Enum
    {
        var value = ReadStringValue(key);
        if (value is null)
        {
            return null;
        }
        return Enum.Parse<T>(value);
    }

    void SaveValue<T>(string key, T value)
        where T : struct, Enum
        => SaveValue(key, value.ToString());
}
