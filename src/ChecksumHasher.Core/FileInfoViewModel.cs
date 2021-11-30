using CommunityToolkit.Mvvm.ComponentModel;
using KozmoTech.ZenUtility.System.IO;

namespace KozmoTech.ZenUtility.ChecksumHasher;

public class FileInfoViewModel : ObservableObject, IContentProvider
{
    public FileInfoViewModel(IFileInfo file) => this.file = file ?? throw new ArgumentNullException(nameof(file));

    public async Task RefreshPropertiesAsync()
    {
        OnPropertyChanging(nameof(FileName));
        OnPropertyChanging(nameof(DirectoryPath));
        OnPropertyChanging(nameof(TotalLength));

        await file.LoadPropertiesAsync();

        OnPropertyChanged(nameof(FileName));
        OnPropertyChanged(nameof(DirectoryPath));
        OnPropertyChanged(nameof(TotalLength));
    }

    async Task<StreamReaderWithProgress> IContentProvider.CreateContentReaderAsync()
    {
        var stream = await file.OpenSequentialReadAsync();
        return new StreamReaderWithProgress(stream);
    }

    public string FileName => Path.GetFileName(file.FullPath) ?? string.Empty;
    public string DirectoryPath => Path.GetDirectoryName(file.FullPath) ?? string.Empty;
    public ulong TotalLength => file.Length;

    private readonly IFileInfo file;
}
