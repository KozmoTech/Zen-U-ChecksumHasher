using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public class FileInfoViewModel : ObservableObject, IContentProvider
{
    public FileInfoViewModel(IFileInfo file) => this.file = file ?? throw new ArgumentNullException(nameof(file));

    public async Task RefreshPropertiesAsync()
    {
        await file.LoadPropertiesAsync();
        OnPropertyChanged(new PropertyChangedEventArgs(null));
    }

    public async Task RefreshNonessentialPropertiesAsync()
    {
        await file.LoadIconAsync();
        OnPropertyChanged(nameof(Icon));
    }

    Task<Stream> IContentProvider.CreateContentReaderAsync() => file.OpenSequentialReadAsync();

    public string FileName => Path.GetFileName(file.FullPath) ?? string.Empty;
    public string DirectoryPath => Path.GetDirectoryName(file.FullPath) ?? string.Empty;
    public string? FileType => file.Type;
    public ulong? TotalLength => file.Length;
    public IPicture? Icon => file.Icon;
    public DateTimeOffset? CreatedAt => file.CreatedAt;
    public DateTimeOffset? ModifiedAt => file.ModifiedAt;

    private readonly IFileInfo file;
}
