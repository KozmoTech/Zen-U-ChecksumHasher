using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

public sealed partial class SettingItemHeader : UserControl
{
    public SettingItemHeader()
    {
        InitializeComponent();
        UpdateLayoutVisualState();
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(SettingItemHeader), null);

    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(UIElement), typeof(SettingItemHeader), null);

    public UIElement? Title
    {
        get => (UIElement?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(nameof(SubTitle), typeof(UIElement), typeof(SettingItemHeader), null);

    public UIElement? SubTitle
    {
        get => (UIElement?)GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(UIElement), typeof(SettingItemHeader), new(null, CommandPropertyChanged));

    public UIElement? Command
    {
        get => (UIElement?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    private static void CommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SettingItemHeader)d).UpdateLayoutVisualState();

    private void UpdateLayoutVisualState()
    {
        VisualStateManager.GoToState(this, Command is null ? nameof(Default) : nameof(Full), false);
    }
}
