using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.Collections;
using KozmoTech.CoreFx.System;
using KozmoTech.ZenUtility.ChecksumHasher.Core;
using KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// A page used to calculate file checksums.
/// </summary>
public sealed partial class FileHasherPage : Page
{
    public FileHasherPage()
    {
        FilteredHashers = new(ViewModel.Hashers, true)
        {
            Filter = x => ((HashCalculatorViewModel)x).IsEnabled,
            SortDescriptions =
            {
                new SortDescription(nameof(HashCalculatorViewModel.Algorithm), SortDirection.Ascending),
            },
        };
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        FilteredHashers.ObserveFilterProperty(nameof(HashCalculatorViewModel.IsEnabled));
        CurrentVisualState = PageVisualState.Initial;
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        FilteredHashers.ClearObservedFilterProperties();
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public FileHasherViewModel ViewModel => Ioc.Default.GetRequiredService<FileHasherViewModel>();

    public AdvancedCollectionView FilteredHashers { get; }

    public static readonly DependencyProperty CurrentVisualStateProperty =
        DependencyProperty.Register(nameof(CurrentVisualState), typeof(PageVisualState), typeof(FileHasherPage), new(PageVisualState.Unknown, CurrentVisualStateChanged));

    public PageVisualState CurrentVisualState
    {
        get => (PageVisualState)GetValue(CurrentVisualStateProperty);
        set => SetValue(CurrentVisualStateProperty, value);
    }

    public static ImageSource ToImageSource(IPicture picture) =>
        picture switch
        {
            null => throw new ArgumentNullException(nameof(picture)),
            BitmapPicture bitmap => bitmap.Image,
            _ => throw new NotSupportedException($"{picture.GetType()} is not supported"),
        };

    public static string PickFileButtonText(PageVisualState state) => state == PageVisualState.Initial ? "Pick a File" : "Pick another File";

    private static void CurrentVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        VisualStateManager.GoToState((FileHasherPage)d, (PageVisualState)e.NewValue switch
        {
            PageVisualState.Initial => nameof(NoFilesSelected),
            PageVisualState.Loading => nameof(LoadingFileMetadata),
            PageVisualState.Computing => nameof(ComputingHash),
            PageVisualState.Computed => nameof(HashCompleted),
            _ => throw new NotSupportedException($"{e.NewValue} state is not supported"),
        }, true);

    // DragEnter and DragLeave will only be fired when RootContent.AllowDrop = True, which is set when "HashCompleted"
    private void RootContent_DragEnter(object sender, DragEventArgs e) => VisualStateManager.GoToState(this, nameof(ShowFileDropperOverlay), true);
    private void RootContent_DragLeave(object sender, DragEventArgs e) => VisualStateManager.GoToState(this, nameof(HideFileDropper), true);

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
        // RootContent.DragLeave will not be fired when file is dropped
        VisualStateManager.GoToState(this, nameof(HideFileDropper), true);

        CurrentVisualState = PageVisualState.Loading;
        ViewModel.FileInfo = new FileInfoViewModel(new WindowsFileInfo(file));

        using (new ScopeDefer(() => CurrentVisualState = PageVisualState.Computing))
        {
            await ViewModel.FileInfo.RefreshPropertiesAsync();
        }

        using (new ScopeDefer(() => CurrentVisualState = PageVisualState.Computed))
        {
            await Task.WhenAll(
                ViewModel.FileInfo.RefreshNonessentialPropertiesAsync(),
                ViewModel.ComputeAllHashesCommand.ExecuteAsync(null));
        }
    }

    public enum PageVisualState
    {
        Unknown, Initial, Loading, Computing, Computed
    }
}
