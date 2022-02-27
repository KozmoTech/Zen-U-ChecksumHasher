using CommunityToolkit.Diagnostics;
using KozmoTech.CoreFx.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LegacyIEnumerable = System.Collections.IEnumerable;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// The UI extension which adds <see cref="ItemsSourceProperty"/> and <see cref="ItemTemplateProperty"/> to <see cref="MenuFlyout"/>.
/// </summary>
/// <remarks>
/// <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/1087"/>
/// </remarks>
public sealed class MenuFlyoutExtension : DependencyObject
{
    /// <summary>
    /// The bindable items collection attached to a <see cref="MenuFlyout"/>.
    /// </summary>
    /// <remarks>
    /// We have to use <see cref="LegacyIEnumerable"/> because XAML cannot assign a strongly typed collection to <see cref="IEnumerable{object}"/>.
    /// </remarks>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.RegisterAttached("ItemsSource", typeof(LegacyIEnumerable), typeof(MenuFlyoutExtension), new(null, OnItemsSourceChanged));

    public static LegacyIEnumerable? GetItemsSource(MenuFlyout d) => (LegacyIEnumerable?)d.GetValue(ItemsSourceProperty);
    public static void SetItemsSource(MenuFlyout d, LegacyIEnumerable? value) => d.SetValue(ItemsSourceProperty, value);

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => RefreshMenuFlyoutItems((MenuFlyout)d);


    /// <summary>
    /// The UI template which generates <see cref="MenuFlyoutItemBase"/>s from <see cref="ItemsSourceProperty"/>.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.RegisterAttached("ItemTemplate", typeof(DataTemplate), typeof(MenuFlyoutExtension), new(null, OnItemTemplateChanged));

    public static DataTemplate? GetItemTemplate(MenuFlyout d) => (DataTemplate?)d.GetValue(ItemTemplateProperty);
    public static void SetItemTemplate(MenuFlyout d, DataTemplate? value) => d.SetValue(ItemTemplateProperty, value);

    private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => RefreshMenuFlyoutItems((MenuFlyout)d);


    private static void RefreshMenuFlyoutItems(MenuFlyout @this)
    {
        var items = GetItemsSource(@this);
        if (items is not null)
        {
            var template = GetItemTemplate(@this);
            Guard.IsNotNull(template, "ItemTemplate");

            @this.Items.ReplaceWith(items.OfType<object?>().Select(obj =>
            {
                var ui = (MenuFlyoutItemBase)template.LoadContent();
                ui.DataContext = obj;
                return ui;
            }));
        }
    }
}
