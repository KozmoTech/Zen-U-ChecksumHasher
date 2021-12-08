using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.UI;
using KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// A page used to calculate and compare file checksums.
/// </summary>
public sealed partial class FileHasherPage : Page, IPageWithHeader
{
    public FileHasherPage()
    {
        InitializeComponent();
        HeaderViewModel = new FileHasherPageHeaderViewModel("File Checksum", ViewModel);
        SortedHashers = new(ViewModel.Hashers, true)
        {
            SortDescriptions =
            {
                new SortDescription(nameof(HashCalculatorViewModel.Algorithm), SortDirection.Ascending),
            },
        };
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public FileHasherViewModel ViewModel => Ioc.Default.GetRequiredService<FileHasherViewModel>();

    public IPageHeaderViewModel HeaderViewModel { get; }
    public DataTemplate HeaderTemplate => PageHeaderTemplate;

    public AdvancedCollectionView SortedHashers { get; }

    public static ImageSource ToImageSource(IPicture picture) =>
        picture switch
        {
            null => throw new ArgumentNullException(nameof(picture)),
            BitmapPicture bitmap => bitmap.Image,
            _ => throw new NotSupportedException($"{picture.GetType()} is not supported"),
        };

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var filePicker = new FileOpenPicker
        {
            FileTypeFilter =
            {
                "*",
            },
        };

        var mainWindow = ((App)Application.Current).MainWindow;
        Debug.Assert(mainWindow is not null);
        var file = await mainWindow.ShowPickSingleFileDialogAsync(filePicker);

        if (file is not null)
        {
            await ComputeHashesAsync(file);
        }
    }

    [SuppressMessage("Performance", "CA1826:Do not use Enumerable methods on indexable collections", Justification = "FirstOrDefault will not hurt much performance")]
    private async void FileDropper_FilesDropped(object sender, FilesDroppedEventArgs e)
    {
        var file = e.Files.FirstOrDefault();
        if (file is not null)
        {
            await ComputeHashesAsync(file);
        }
    }

    private async Task ComputeHashesAsync(StorageFile file)
    {
        ViewModel.FileInfo = new FileInfoViewModel(new WindowsFileInfo(file));
        await ViewModel.FileInfo.RefreshPropertiesAsync();
        await Task.WhenAll(
            ViewModel.FileInfo.RefreshNonessentialPropertiesAsync(),
            ViewModel.ComputeAllHashesCommand.ExecuteAsync(null));
    }
}

public sealed record class FileHasherPageHeaderViewModel(string Title, FileHasherViewModel PageViewModel) : PageHeaderViewModel(Title);
