using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;

namespace KozmoTech.ZenUtility.ChecksumHasher;

/// <summary>
/// The UI App specific settings.
/// </summary>
public sealed partial class AppSettingsViewModel : SettingsViewModel
{
    public AppSettingsViewModel() : base(AppSettingsStorage.Default)
    {
        uiContainer = Storage.RootContainer.EnsureSubContainer(UiSettingsContainerKey);
    }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public string AppName => Package.Current.DisplayName;

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public PackageVersion AppVersion => Package.Current.Id.Version;

    /// <summary>
    /// The current active UI theme applied to the app.
    /// </summary>
    public ElementTheme UiTheme
    {
        get => uiContainer.ReadEnumValue<ElementTheme>(ThemeSettingKey) ?? ElementTheme.Default;
        set
        {
            if (value != UiTheme)
            {
                if (value == ElementTheme.Default)
                {
                    uiContainer.DeleteValue(ThemeSettingKey);
                }
                else
                {
                    uiContainer.SaveValue(ThemeSettingKey, value);
                }
                OnPropertyChanged();
            }
        }
    }

    private readonly ISettingsStorageContainer uiContainer;

    private const string UiSettingsContainerKey = "GUI";
    private const string ThemeSettingKey = "Theme";
}
