using Microsoft.UI.Xaml;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

public interface IPageHeaderViewModel
{
    string Title { get; }
}

public interface IPageWithHeader
{
    IPageHeaderViewModel HeaderViewModel { get; }
    DataTemplate HeaderTemplate { get; }
}

public record class PageHeaderViewModel(string Title) : IPageHeaderViewModel;
