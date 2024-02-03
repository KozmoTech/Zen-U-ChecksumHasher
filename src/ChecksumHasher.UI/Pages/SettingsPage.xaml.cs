using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;

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

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
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
