using Microsoft.UI.Xaml.Data;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal abstract class SingleValueConverter<TSource, TTarget> : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (!typeof(TTarget).IsAssignableTo(targetType))
        {
            throw new ArgumentException($"{targetType} is not supported", nameof(targetType));
        }
        if (value is null)
        {
            return ConvertCore(default);
        }
        if (value is not TSource source)
        {
            throw new ArgumentException($"{nameof(value)}'s type {value?.GetType()} is not supported", nameof(value));
        }
        return ConvertCore(source);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        if (!typeof(TSource).IsAssignableTo(targetType))
        {
            throw new ArgumentException($"{targetType} is not supported", nameof(targetType));
        }
        if (value is null)
        {
            return ConvertBackCore(default);
        }
        if (value is not TTarget target)
        {
            throw new ArgumentException($"{nameof(value)}'s type {value?.GetType()} is not supported", nameof(value));
        }
        return ConvertBackCore(target);
    }

    protected abstract TTarget? ConvertCore(TSource? value);

    protected abstract TSource? ConvertBackCore(TTarget? value);
}
