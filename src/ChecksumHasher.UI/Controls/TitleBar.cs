using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Graphics;
using Windows.UI;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;

public interface ITitleBarInteractiveControlsProvider
{
    IEnumerable<FrameworkElement> ListTitleBarInteractiveControls();
}

public sealed class TitleBar : Control
{
    public TitleBar()
    {
        DefaultStyleKey = typeof(TitleBar);
        Loaded += (s, e) => UpdateInteractiveControlsRegions();
        SizeChanged += (s, e) => UpdateInteractiveControlsRegions();
    }

    /// <summary>
    /// Setup the title bar customization for the specific <paramref name="window"/>.
    /// We recommend you to call this function in <paramref name="window"/>'s constructor, with <see cref="Window.Title"/> property set.
    /// </summary>
    /// <param name="window">The main window whose title bar would be customized.</param>
    /// <param name="controls">The interactive controls that might locate within the title bar area.</param>
    public void ReplaceSystemTitleBar(Window window, params ITitleBarInteractiveControlsProvider[] controls)
    {
        Title = window.Title;
        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(this);

        _sysTitleBar = window.AppWindow.TitleBar;
        _interactiveControls = controls;
        _ncHitTestRegions = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);

        _sysTitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        UpdateTitleBarButtonColors();
    }

    #region Title

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(TitleBar), new(string.Empty));

    /// <summary>
    /// Get or set the title text.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion Title

    #region Button Colors

    public static readonly DependencyProperty ButtonForegroundColorProperty =
        DependencyProperty.Register(nameof(ButtonForegroundColor), typeof(Color?), typeof(TitleBar), new(null, ButtonColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons foreground color. <c>new Color()</c> means default value.
    /// </summary>
    public Color? ButtonForegroundColor
    {
        get => (Color?)GetValue(ButtonForegroundColorProperty);
        set => SetValue(ButtonForegroundColorProperty, value);
    }

    public static readonly DependencyProperty ButtonHoverColorProperty = DependencyProperty.Register(nameof(ButtonHoverColor), typeof(Color?), typeof(TitleBar), new(null, ButtonColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons hover background color. <c>new Color()</c> means default value.
    /// </summary>
    public Color? ButtonHoverColor
    {
        get => (Color?)GetValue(ButtonHoverColorProperty);
        set => SetValue(ButtonHoverColorProperty, value);
    }

    public static readonly DependencyProperty ButtonPressedColorProperty =
        DependencyProperty.Register(nameof(ButtonPressedColor), typeof(Color?), typeof(TitleBar), new(null, ButtonColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons pressed background color. <c>new Color()</c> means default value.
    /// </summary>
    public Color? ButtonPressedColor
    {
        get => (Color?)GetValue(ButtonPressedColorProperty);
        set => SetValue(ButtonPressedColorProperty, value);
    }

    private static void ButtonColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TitleBar)d).UpdateTitleBarButtonColors();

    private void UpdateTitleBarButtonColors()
    {
        if (_sysTitleBar != null)
        {
            _sysTitleBar.ButtonForegroundColor = ButtonForegroundColor;
            _sysTitleBar.ButtonHoverForegroundColor = ButtonForegroundColor;
            _sysTitleBar.ButtonHoverBackgroundColor = ButtonHoverColor;
            _sysTitleBar.ButtonPressedForegroundColor = ButtonForegroundColor;
            _sysTitleBar.ButtonPressedBackgroundColor = ButtonPressedColor;
        }
    }

    #endregion Button Colors

    #region Interactive Controls

    private InputNonClientPointerSource? _ncHitTestRegions;
    private IEnumerable<ITitleBarInteractiveControlsProvider>? _interactiveControls;

    private void UpdateInteractiveControlsRegions()
    {
        Debug.Assert(_ncHitTestRegions is not null);
        Debug.Assert(_interactiveControls is not null);

        var scale = XamlRoot.RasterizationScale;
        var passRegions = from pvd in _interactiveControls
                          from c in pvd.ListTitleBarInteractiveControls()
                          select TransformToNativeNCRect(c);
        _ncHitTestRegions.SetRegionRects(NonClientRegionKind.Passthrough, passRegions.ToArray());

        RectInt32 TransformToNativeNCRect(FrameworkElement element)
        {
            var coordinate = element.TransformToVisual(null);
            var bounds = coordinate.TransformBounds(new(
                x: 0, y: 0,
                width: element.ActualWidth,
                height: element.ActualHeight));
            return new(
                _X: RoundWithScale(bounds.X),
                _Y: RoundWithScale(bounds.Y),
                _Width: RoundWithScale(bounds.Width),
                _Height: RoundWithScale(bounds.Height));

            int RoundWithScale(double x) => (int)Math.Round(x * scale);
        }
    }

    #endregion Interactive Controls

    private AppWindowTitleBar? _sysTitleBar;
}
