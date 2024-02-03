using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// A strongly typed enumeration representing all supported pages in this App.
/// </summary>
public enum AppPage
{
    FileHasher, HashVerifier, Settings
}

internal static class AppPageExtensions
{
    internal static Type ToPageType(this AppPage @this) =>
        @this switch
        {
            AppPage.FileHasher => typeof(FileHasherPage),
            AppPage.HashVerifier => typeof(HashVerifierPage),
            AppPage.Settings => typeof(SettingsPage),
            _ => throw new NotSupportedException($"page {@this} is not supported"),
        };
}

/// <summary>
/// MainWindow handles all root level navigation related stuffs.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Title = SettingsViewModel.AppName;
        MainTitleBar.ReplaceSystemTitleBar(this, MainNavigation);
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public AppSettingsViewModel SettingsViewModel => Ioc.Default.GetRequiredService<AppSettingsViewModel>();

    public IAsyncOperation<StorageFile?> ShowPickSingleFileDialogAsync(FileOpenPicker dialog)
    {
        InitializeWithWindow.Initialize(dialog, WindowNative.GetWindowHandle(this));
        return dialog.PickSingleFileAsync();
    }

    #region Navigation

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavigateTo(AppPage.Settings);
        }
        else if (args.SelectedItemContainer is not null)
        {
            NavigateTo((AppPage)args.SelectedItemContainer.Tag);
        }

        void NavigateTo(AppPage targetPage)
        {
            if (targetPage != _currentPage)
            {
                ContentFrame.Navigate(targetPage.ToPageType());
                _currentPage = targetPage;
            }
        }
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        MainNavigation.IsBackEnabled = ContentFrame.CanGoBack;
        MainNavigation.SelectedItem = e.SourcePageType == typeof(SettingsPage) ? MainNavigation.SettingsItem : FindFirstByTag(e.SourcePageType);

        NavigationViewItem FindFirstByTag(Type pageType) =>
            (from it in MainNavigation.MenuItems
             where it is NavigationViewItem
             let nvi = (NavigationViewItem)it
             where ((AppPage)nvi.Tag).ToPageType() == pageType
             select nvi).First();
    }

    private void MainNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }

    private AppPage? _currentPage = null;

    #endregion Navigation

}
