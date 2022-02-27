using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public class SettingsViewModel : ObservableObject
{
    public SettingsViewModel(ISettingsStorage storage) => Storage = storage ?? throw new ArgumentNullException(nameof(storage));

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Simpler for bindings")]
    public Version CoreVersion => typeof(SettingsViewModel).Assembly.GetName().Version ?? throw new NotSupportedException("no core version available");

    protected ISettingsStorage Storage { get; }
}
