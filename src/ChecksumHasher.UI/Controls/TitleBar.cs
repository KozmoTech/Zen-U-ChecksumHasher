using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Graphics;
using Windows.UI;
using WinRT.Interop;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;

public sealed class TitleBar : Control
{
    public TitleBar()
    {
        DefaultStyleKey = typeof(TitleBar);
        SizeChanged += TitleBar_SizeChanged;
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(TitleBar), new PropertyMetadata(string.Empty, TitleChanged));

    /// <summary>
    /// Get or set the title of the window.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty ButtonForegroundColorProperty =
        DependencyProperty.Register(nameof(ButtonForegroundColor), typeof(Color), typeof(TitleBar), new PropertyMetadata(null, ButtonForegroundColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons foreground color. <c>new Color()</c> means default value.
    /// </summary>
    public Color ButtonForegroundColor
    {
        get => (Color)GetValue(ButtonForegroundColorProperty);
        set => SetValue(ButtonForegroundColorProperty, value);
    }

    public static readonly DependencyProperty ButtonHoverColorProperty =
        DependencyProperty.Register(nameof(ButtonHoverColor), typeof(Color), typeof(TitleBar), new PropertyMetadata(null, ButtoHoverColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons hover background color. <c>new Color()</c> means default value.
    /// </summary>
    public Color ButtonHoverColor
    {
        get => (Color)GetValue(ButtonHoverColorProperty);
        set => SetValue(ButtonHoverColorProperty, value);
    }

    public static readonly DependencyProperty ButtonPressedColorProperty =
        DependencyProperty.Register(nameof(ButtonPressedColor), typeof(Color), typeof(TitleBar), new PropertyMetadata(null, ButtoPressedColorChanged));

    /// <summary>
    /// Get or set the Min-Max buttons pressed background color. <c>new Color()</c> means default value.
    /// </summary>
    public Color ButtonPressedColor
    {
        get => (Color)GetValue(ButtonPressedColorProperty);
        set => SetValue(ButtonPressedColorProperty, value);
    }

    /// <summary>
    /// Setup the title bar customization for the specific <paramref name="window"/>.
    /// </summary>
    /// <param name="window">The main window whose title bar would be customized.</param>
    /// <remarks>
    /// We did not use <see cref="Window.ExtendsContentIntoTitleBar"/> because the implementation now is buggy. For example,
    /// here is <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/6333">one of the various issues</seealso>.
    /// Besides, the titlebar background and the window shadow is far from normal as well.
    /// </remarks>
    public void ReplaceSystemTitleBar(Window window)
    {
        hwnd = WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        appWindow = AppWindow.GetFromWindowId(windowId);
        rootContent = window.Content;

        ExtendContentsIntoTitleBar();
    }

    private void ExtendContentsIntoTitleBar()
    {
        Debug.Assert(hwnd != IntPtr.Zero && appWindow is not null);

        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        UpdateWindowTitle();
        UpdateTitleBarButtonForegroundColor();
        UpdateTitleBarButtonHoverColor();
        UpdateTitleBarButtonPressedColor();
    }

    private void UpdateTitleBarDragRegion()
    {
        Debug.Assert(appWindow is not null && rootContent is not null);

        var coordinateTransform = TransformToVisual(rootContent);
        var originalRegion = new Rect(new Point(0, 0), new Size(ActualSize.X, ActualSize.Y));
        var targetRegion = coordinateTransform.TransformBounds(originalRegion);
        appWindow.TitleBar.SetDragRectangles(new[]
        {
            new RectInt32((int)targetRegion.X, (int)targetRegion.Y, (int)targetRegion.Width, (int)targetRegion.Height),
        });
    }

    private void UpdateWindowTitle()
    {
        if (appWindow is not null)
        {
            appWindow.Title = Title;
        }
    }

    private void UpdateTitleBarButtonForegroundColor()
    {
        if (appWindow is not null)
        {
            appWindow.TitleBar.ButtonForegroundColor
                = appWindow.TitleBar.ButtonHoverForegroundColor
                = appWindow.TitleBar.ButtonPressedForegroundColor
                = ToTitleBarColor(ButtonForegroundColor);
        }
    }

    private void UpdateTitleBarButtonHoverColor()
    {
        if (appWindow is not null)
        {
            // TODO: can use CommunityToolkit.WinUI.UI.TitleBarExtensions here
            appWindow.TitleBar.ButtonHoverBackgroundColor = ToTitleBarColor(ButtonHoverColor);
        }
    }

    private void UpdateTitleBarButtonPressedColor()
    {
        if (appWindow is not null)
        {
            appWindow.TitleBar.ButtonPressedBackgroundColor = ToTitleBarColor(ButtonPressedColor);
        }
    }

    private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TitleBar) d).UpdateWindowTitle();

    private static void ButtonForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TitleBar)d).UpdateTitleBarButtonForegroundColor();

    private static void ButtoHoverColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TitleBar)d).UpdateTitleBarButtonHoverColor();

    private static void ButtoPressedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TitleBar)d).UpdateTitleBarButtonPressedColor();

    private void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateTitleBarDragRegion();

    private static Color? ToTitleBarColor(Color color) => color == EmptyColor ? null : color;

    private IntPtr hwnd = IntPtr.Zero;
    private AppWindow? appWindow;
    private UIElement? rootContent;

    private static readonly Color EmptyColor = new Color();
}
