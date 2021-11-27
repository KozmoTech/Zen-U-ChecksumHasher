using CommunityToolkit.Mvvm.DependencyInjection;
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
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// A page hosting application runtime settings.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    public AppSettingsViewModel ViewModel => Ioc.Default.GetRequiredService<AppSettingsViewModel>();

    public static string FormatThemeDescription(ElementTheme theme) =>
        theme switch
        {
            ElementTheme.Light => "Light",
            ElementTheme.Dark => "Dark",
            _ => "Using system setting",
        };

    public static string FormatAppNameAndVersion(string appName, PackageVersion version, Version coreVersion) =>
        $"{appName} {version.Major}.{version.Minor}.{version.Build}.{version.Revision}      (Core Library {coreVersion})";
}
