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
/// MainWindow handles all root level navigation related stuffs.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        hwnd = WindowNative.GetWindowHandle(this);
        MainTitleBar.ReplaceSystemTitleBar(this);
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public AppSettingsViewModel SettingsViewModel => Ioc.Default.GetRequiredService<AppSettingsViewModel>();

    public IAsyncOperation<StorageFile?> ShowPickSingleFileDialogAsync(FileOpenPicker dialog)
    {
        InitializeWithWindow.Initialize(dialog, hwnd);
        return dialog.PickSingleFileAsync();
    }

    private void MainNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavigateTo(SupportedPageType.Settings);
        }
        else if (args.SelectedItemContainer is not null)
        {
            NavigateTo((SupportedPageType)args.SelectedItemContainer.Tag);
        }

        void NavigateTo(SupportedPageType targetPage)
        {
            if (targetPage != currentPage)
            {
                ContentFrame.Navigate(SupportedPageTypeToPage(targetPage));
                currentPage = targetPage;
            }
        }
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        MainNavigation.IsBackEnabled = ContentFrame.CanGoBack;

        MainNavigation.Header = null;
        if (ContentFrame.Content is IPageWithHeader pageWithHeader)
        {
            MainNavigation.Header = pageWithHeader.HeaderViewModel;
            MainNavigation.HeaderTemplate = pageWithHeader.HeaderTemplate;
        }

        MainNavigation.SelectedItem = PageToSupportedPageType(e.SourcePageType) switch
        {
            SupportedPageType.Settings => MainNavigation.SettingsItem,
            SupportedPageType pageType => MainNavigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => (SupportedPageType)x.Tag == pageType),
        };
    }

    private void MainNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e) => TryGoBack();

    private bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
        {
            return false;
        }
        if (MainNavigation.IsPaneOpen)
        {
            // "Go Back" means close the navigation menu
            MainNavigation.IsPaneOpen = false;
            return false;
        }

        ContentFrame.GoBack();
        return true;
    }

    private static Type SupportedPageTypeToPage(SupportedPageType page) =>
        page switch
        {
            SupportedPageType.FileHasher => typeof(FileHasherPage),
            SupportedPageType.Settings => typeof(SettingsPage),
            _ => throw new NotSupportedException($"page {page} is not supported"),
        };

    private static SupportedPageType PageToSupportedPageType(Type page) =>
        page == typeof(FileHasherPage) ? SupportedPageType.FileHasher
        : page == typeof(SettingsPage) ? SupportedPageType.Settings
        : throw new NotSupportedException($"page {page} is not recognized");

    private readonly IntPtr hwnd;
    private SupportedPageType? currentPage = null;
}

/// <summary>
/// A strongly typed enumeration representing all supported pages in this App.
/// </summary>
public enum SupportedPageType
{
    FileHasher, Settings
}
