namespace KozmoTech.ZenUtility.ChecksumHasher;

internal abstract class NonNullSingleValueConverter<TSource, TTarget> : SingleValueConverter<TSource, TTarget>
{
    protected NonNullSingleValueConverter(TTarget convertNullFallback, TSource convertBackNullFallback)
    {
        ConvertNullFallback = convertNullFallback ?? throw new ArgumentNullException(nameof(convertNullFallback));
        ConvertBackNullFallback = convertBackNullFallback ?? throw new ArgumentNullException(nameof(convertNullFallback));
    }

    public TTarget ConvertNullFallback { get; }

    public TSource ConvertBackNullFallback { get; }

    protected override TTarget? ConvertCore(TSource? value) => value is null ? ConvertNullFallback : ConvertCoreNotNull(value);

    protected override TSource? ConvertBackCore(TTarget? value) => value is null ? ConvertBackNullFallback : ConvertBackCoreNotNull(value);

    protected abstract TTarget ConvertCoreNotNull(TSource value);

    protected abstract TSource ConvertBackCoreNotNull(TTarget value);
}
