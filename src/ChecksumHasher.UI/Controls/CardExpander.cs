using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI.Controls;

/// <summary>
/// A card-like expander which is not expandable when Content is <c>null</c> (similar to the one in Windows 11 Settings).
/// </summary>
[TemplateVisualState(Name = HeaderExpandableState, GroupName = "ExpandableStates")]
[TemplateVisualState(Name = HeaderPlainState, GroupName = "ExpandableStates")]
public sealed class CardExpander : Expander
{
    public CardExpander()
    {
        // https://github.com/microsoft/microsoft-ui-xaml/issues/3502
        DefaultStyleResourceUri = new Uri("ms-appx:///Themes/Generic.xaml");
        DefaultStyleKey = typeof(CardExpander);
    }

    protected override void OnApplyTemplate()
    {
        UpdateHeaderExpandableState();
        base.OnApplyTemplate();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        UpdateHeaderExpandableState();
    }

    private void UpdateHeaderExpandableState()
    {
        if (Content is null)
        {
            IsExpanded = false;
            VisualStateManager.GoToState(this, HeaderPlainState, false);
        }
        else
        {
            VisualStateManager.GoToState(this, HeaderExpandableState, false);
        }
    }

    private const string HeaderExpandableState = "Expandable";
    private const string HeaderPlainState = "Plain";
}
