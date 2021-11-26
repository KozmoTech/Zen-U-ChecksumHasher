using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace KozmoTech.ZenUtility.ChecksumHasher;

/// <summary>
/// The UI App specific settings.
/// </summary>
public sealed partial class AppSettingsViewModel : SettingsViewModel
{
    public string AppName => Package.Current.DisplayName;

    public PackageVersion AppVersion => Package.Current.Id.Version;

    /// <summary>
    /// The current active UI theme applied to the app.
    /// </summary>
    [ObservableProperty]
    private ElementTheme uiTheme = ElementTheme.Default;

    /// <summary>
    /// The current selected theme, which will update the non-<see cref="Nullable"/> <see cref="UITheme"/>. Do not use this property unless in UI bindings.
    /// </summary>
    /// <remarks>
    /// We must use <see cref="object"/> here because there is a <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/3268">BUG in WinUI</seealso>;
    /// so we have to use <see cref="Nullable"/>, however {x:Bind} does not recognize <see cref="Nullable"/>.
    /// </remarks>
    public object SelectedTheme
    {
        get => UiTheme;
        set
        {
            if (value is ElementTheme uiTheme && uiTheme != UiTheme)
            {
                OnPropertyChanging();
                UiTheme = uiTheme;
                OnPropertyChanged();
            }
        }
    }
}
