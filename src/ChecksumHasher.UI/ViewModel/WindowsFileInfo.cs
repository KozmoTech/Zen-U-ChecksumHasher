using Windows.Storage;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal class WindowsFileInfo : IFileInfo
{
    public WindowsFileInfo(StorageFile file) => this.file = file ?? throw new ArgumentNullException(nameof(file));

    public string FullPath => file.Path;
    public ulong Length { get; private set; }

    public async Task LoadPropertiesAsync()
    {
        var props = await file.GetBasicPropertiesAsync();
        Length = props.Size;
    }

    public Task<Stream> OpenSequentialReadAsync() => file.OpenStreamForReadAsync();

    private readonly StorageFile file;
}
