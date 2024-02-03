using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;

public sealed class TitleBarTopNavigationView : NavigationView, ITitleBarInteractiveControlsProvider
{
    public TitleBarTopNavigationView()
    {
        DefaultStyleKey = typeof(NavigationView);
        PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        AlignMenuItemsAndFooter();
    }

    #region Center MenuItems

    private const string TopNavRootGridName = "TopNavGrid";
    private const string FooterContentControlName = "PaneFooterOnTopPane";
    private const string FooterHostControlName = "TopFooterMenuItemsHost";

    private const int MenuItemsHostGridColumn = 3;
    private const int CustomContentGridColumn = 5;

    private static readonly Thickness TitleBarButtonsAwarePadding = new(160, 0, 160, 0);

    private ItemsRepeaterScrollHost? _menuItemsHost;
    private ContentControl? _footerContent;
    private ItemsRepeater? _footersHost;

    /// <summary>
    /// Horizontally center <see cref="NavigationView.MenuItems"/> for this <see cref="NavigationViewPaneDisplayMode.Top"/> <see cref="NavigationView"/>.
    /// </summary>
    /// <remarks>
    /// <para>This implementation utilizes the <seealso href="https://github.com/microsoft/microsoft-ui-xaml/blob/winui3/release/1.4.4/controls/dev/NavigationView/NavigationView.xaml">
    /// WinUI 3 (1.4.4) NavigationView's default template</seealso> as a foundation.</para>
    /// <para>Modifications are made to <see cref="NavigationView.MenuItems"/>'s root <see cref="Grid"/>'s <see cref="ColumnDefinition"/>s to achieve the horizontally-center alignment.</para>
    /// <para>Additionally, we add left and right paddings to the root <see cref="Grid"/> to accommodate the title bar's icon and title (left), and min-max-close buttons (right).</para>
    /// <para>Finally, the right-aligned <see cref="NavigationView.FooterMenuItems"/> is moved up for better alignment with the min-max-close buttons.</para>
    /// </remarks>
    private void AlignMenuItemsAndFooter()
    {
        var rootGrid = (GetTemplateChild(TopNavRootGridName) as Grid)
            ?? throw new InvalidOperationException($"cannot find {TopNavRootGridName}");

        // reset Width from "*" to "Auto"
        rootGrid.ColumnDefinitions[CustomContentGridColumn].MinWidth = 0;
        rootGrid.ColumnDefinitions[CustomContentGridColumn].Width = GridLength.Auto;

        // set Width from "Auto" to "*"
        rootGrid.ColumnDefinitions[MenuItemsHostGridColumn].Width = new GridLength(1, GridUnitType.Star);

        // and horizontally-center align the menu items host
        _menuItemsHost = rootGrid.FindChild<ItemsRepeaterScrollHost>(h => Grid.GetColumn(h) == MenuItemsHostGridColumn)
            ?? throw new InvalidOperationException($"cannot find ItemsRepeaterScrollHost");
        _menuItemsHost.HorizontalAlignment = HorizontalAlignment.Center;

        // leave some leading and trailing space for title bar elements
        rootGrid.Padding = TitleBarButtonsAwarePadding;

        _footerContent = (GetTemplateChild(FooterContentControlName) as ContentControl)
            ?? throw new InvalidOperationException($"cannot find {FooterContentControlName}");
        _footersHost = (GetTemplateChild(FooterHostControlName) as ItemsRepeater)
            ?? throw new InvalidOperationException($"cannot find {FooterHostControlName}");
    }

    #endregion Center MenuItems

    #region TitleBar Interaction

    public IEnumerable<FrameworkElement> ListTitleBarInteractiveControls()
    {
        Debug.Assert(_menuItemsHost is not null);
        Debug.Assert(_footerContent is not null);
        Debug.Assert(_footersHost is not null);
        yield return _menuItemsHost;
        yield return _footerContent;
        yield return _footersHost;
    }

    #endregion TitleBar Interaction
}
