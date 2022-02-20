using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;

public sealed class FilesDroppedEventArgs : EventArgs
{
    public IReadOnlyList<StorageFile> Files { get; init; } = new List<StorageFile>().AsReadOnly();
}

public class FileDropper : ContentControl
{
    public FileDropper()
    {
        DefaultStyleKey = typeof(FileDropper);
        AllowDrop = true;
    }

    public event EventHandler<FilesDroppedEventArgs>? FilesDropped;

    public static readonly DependencyProperty AllowMultipleFilesProperty = DependencyProperty.Register(nameof(AllowMultipleFiles), typeof(bool), typeof(FileDropper), new(false));

    public bool AllowMultipleFiles
    {
        get => (bool)GetValue(AllowMultipleFilesProperty);
        set => SetValue(AllowMultipleFilesProperty, value);
    }

    /// <summary>
    /// Decides whether we accept the dragging contents.
    /// </summary>
    /// <param name="e">The customizable dragging feedback provided by WinUI.</param>
    /// <remarks>
    /// Known issue: when you dragging a file and quickly moves in and out, it might fail randomly.
    /// Here is the <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/4574">WinUI issue</seealso> to track.
    /// </remarks>
    protected override async void OnDragEnter(DragEventArgs e)
    {
        var deferral = e.GetDeferral();
        e.Handled = true;
        try
        {
            e.AcceptedOperation = DataPackageOperation.None;
            var items = await e.DataView.GetStorageItemsAsync();
            if (AreItemsAllowed(items))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.IsGlyphVisible = false;
                e.DragUIOverride.IsCaptionVisible = false;
            }

            base.OnDragEnter(e);

            if (e.AcceptedOperation != DataPackageOperation.None)
            {
                VisualStateManager.GoToState(this, AcceptingDragVisualState, true);
            }
            e.Handled = true;
        }
        finally
        {
            deferral?.Complete();
        }
    }

    protected override void OnDragLeave(DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.None;
        VisualStateManager.GoToState(this, NormalVisualState, true);
        base.OnDragLeave(e);
        e.Handled = true;
    }

    protected override async void OnDrop(DragEventArgs e)
    {
        var deferral = e.GetDeferral();
        e.Handled = true;
        try
        {
            VisualStateManager.GoToState(this, NormalVisualState, true);

            base.OnDrop(e);

            if (e.AcceptedOperation != DataPackageOperation.None)
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (AreItemsAllowed(items))
                {
                    OnFilesDropped(new FilesDroppedEventArgs
                    {
                        Files = (from x in items
                                 let f = x as StorageFile
                                 where f is not null
                                 select f).ToList().AsReadOnly(),
                    });
                }
            }
            e.Handled = true;
        }
        finally
        {
            deferral?.Complete();
        }
    }

    protected virtual void OnFilesDropped(FilesDroppedEventArgs e) => FilesDropped?.Invoke(this, e);

    private bool AreItemsAllowed(IReadOnlyList<IStorageItem>? items) =>
        (items is { Count: 1 } || (items is { Count: > 1 } && AllowMultipleFiles))
        && items.All(x => x.IsOfType(StorageItemTypes.File));

    private const string NormalVisualState = "Normal";
    private const string AcceptingDragVisualState = "DragOver";
}
