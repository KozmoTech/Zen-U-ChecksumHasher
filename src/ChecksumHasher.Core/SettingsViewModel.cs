using CommunityToolkit.Mvvm.ComponentModel;

namespace KozmoTech.ZenUtility.ChecksumHasher;

public class SettingsViewModel : ObservableObject
{
    public Version CoreVersion => typeof(SettingsViewModel).Assembly.GetName().Version ?? throw new NotSupportedException("no core version available");
}
