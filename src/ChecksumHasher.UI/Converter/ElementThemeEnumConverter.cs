using Microsoft.UI.Xaml;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

/// <summary>
/// If the value is not null, do nothing; if it is null, return the fallback non-null value.
/// </summary>
/// <typeparam name="T">The <c>enum</c> type to be used.</typeparam>
/// <remarks>
/// We need this converter because there is a <seealso href="https://github.com/microsoft/microsoft-ui-xaml/issues/3268">BUG in WinUI</seealso>:
/// a two-way {x:Bind} for <c>SelectedItem</c> will return a <see cref="Nullable"/> enum.
/// (P.S., bind to a function is not reusable)
/// </remarks>
internal abstract class NullableEnumToEnumConverter<T> : NonNullSingleValueConverter<T, T>
    where T : struct, Enum
{
    protected NullableEnumToEnumConverter(T fallback) : base(fallback, fallback)
    {
    }

    protected override T ConvertCoreNotNull(T value) => value;
    protected override T ConvertBackCoreNotNull(T value) => value;
}

/// <summary>
/// A converter specifically used as the converter for an <see cref="ElementTheme"/> <c>SelectedItem</c> binding.
/// </summary>
internal sealed class ElementThemeSelectionConverter : NullableEnumToEnumConverter<ElementTheme>
{
    public ElementThemeSelectionConverter() : base(ElementTheme.Default)
    {
    }
}
