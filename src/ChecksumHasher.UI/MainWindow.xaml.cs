using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// MainWindow handles all root level navigation related stuffs.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            NavigateTo(SupportedPageType.Settings);
        }
        else if (args.SelectedItemContainer is not null)
        {
            NavigateTo((SupportedPageType) args.SelectedItemContainer.Tag);
        }
    }

    private void NavigateTo(SupportedPageType targetPage)
    {
        if (targetPage != currentPage)
        {
            ContentFrame.Navigate(targetPage switch
            {
                SupportedPageType.FileHasher => typeof(FileHasherPage),
                SupportedPageType.Settings => typeof(SettingsPage),
                _ => throw new NotSupportedException($"page {targetPage} is not supported"),
            });
            currentPage = targetPage;
        }
    }

    private SupportedPageType? currentPage = null;
}

/// <summary>
/// A strongly typed enumeration representing all supported pages in this App.
/// </summary>
public enum SupportedPageType
{
    FileHasher, Settings
}
