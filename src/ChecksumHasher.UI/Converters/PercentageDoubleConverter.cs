namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

internal sealed class PercentageDoubleConverter : NonNullSingleValueConverter<double, double>
{
    public PercentageDoubleConverter() : base(0.0, 0.0)
    {
    }

    protected override double ConvertCoreNotNull(double value) => Math.Clamp(value, 0.0, 1.0);
    protected override double ConvertBackCoreNotNull(double value) => value;
}
