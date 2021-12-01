using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal class WindowsFileInfo : IFileInfo
{
    public WindowsFileInfo(StorageFile file) => this.file = file ?? throw new ArgumentNullException(nameof(file));

    public string FullPath => file.Path;
    public ulong? Length { get; private set; }
    public string? Type => file.DisplayType;
    public IPicture? Icon { get; private set; }
    public DateTimeOffset? CreatedAt => file.DateCreated;
    public DateTimeOffset? ModifiedAt { get; private set; }

    public async Task LoadPropertiesAsync()
    {
        var props = await file.GetBasicPropertiesAsync();
        Length = props.Size;
        ModifiedAt = props.DateModified;
    }

    public async Task LoadIconAsync()
    {
        using var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, IconSize, ThumbnailOptions.UseCurrentScale);
        var image = new BitmapImage();
        await image.SetSourceAsync(thumbnail);
        Icon = new BitmapPicture(image);
    }

    public Task<Stream> OpenSequentialReadAsync() => file.OpenStreamForReadAsync();

    private readonly StorageFile file;

    private const uint IconSize = 256;
}
