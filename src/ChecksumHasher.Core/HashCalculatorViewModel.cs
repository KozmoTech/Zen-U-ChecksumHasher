using CommunityToolkit.Mvvm.ComponentModel;
using KozmoTech.ZenUtility.System;
using KozmoTech.ZenUtility.System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace KozmoTech.ZenUtility.ChecksumHasher;

public abstract partial class HashCalculatorViewModel : ObservableObject, IDisposable
{
    protected HashCalculatorViewModel(HashAlgorithmType algorithm, HashAlgorithm hasher)
    {
        disposable = new(this);
        Algorithm = algorithm;
        this.hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Called in disposable.DoDispose")]
    public void Dispose() => disposable.DoDispose(hasher);

    public HashAlgorithmType Algorithm { get; }

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HashCodeString))]
    private HashStringFormat hashCodeFormat = HashStringFormat.LowerCaseNoDash;

    public double? ComputeProgress
    {
        get => progress;
        private set => SetProperty(ref progress, value);
    }

    public byte[]? HashCode
    {
        get => hashCode;
        private set
        {
            SetProperty(ref hashCode, value);
            OnPropertyChanged(nameof(HashCodeString));
        }
    }

    public string? HashCodeString => HashCode is null ? null : FormatHashCodeString(HashCode, HashCodeFormat);

    internal async Task ComputeHashAsync(IContentProvider content, CancellationToken cancellation = default)
    {
        using var reader = await content.CreateContentReaderAsync();

        ComputeProgress = 0;
        using var defer = new ScopeDefer(() => ComputeProgress = null);
        HashCode = null;
        
        try
        {
            var runner = new TaskRunnerWithTimer(ProgressChangeInterval, Runner_TimerCallback);
            HashCode = await runner.RunAsync(hasher.ComputeHashAsync(reader, cancellation));
        }
        catch (TaskCanceledException)
        {
        }

        // reader.Length might not be correct for a huge file (> 130 GB) in the first several seconds
        void Runner_TimerCallback() => ComputeProgress = (double)reader.LengthRead / content.TotalLength;
    }

    private static string FormatHashCodeString(byte[] data, HashStringFormat format) =>
        format switch
        {
            HashStringFormat.LowerCaseNoDash => string.Join("", from b in data select b.ToString("x2")),
            HashStringFormat.UpperCaseNoDash => string.Join("", from b in data select b.ToString("X2")),
            _ => BitConverter.ToString(data),
        };

    private readonly DisposableHelper disposable;
    private readonly HashAlgorithm hasher;
    private double? progress = null;
    private byte[]? hashCode = null;

    private static readonly TimeSpan ProgressChangeInterval = TimeSpan.FromSeconds(0.6);
}

public sealed class MD5CalculatorViewModel : HashCalculatorViewModel
{
    public MD5CalculatorViewModel() : base(HashAlgorithmType.MD5, MD5.Create())
    {
    }
}

public sealed class SHA1CalculatorViewModel : HashCalculatorViewModel
{
    public SHA1CalculatorViewModel() : base(HashAlgorithmType.SHA1, SHA1.Create())
    {
    }
}

public sealed class SHA256CalculatorViewModel : HashCalculatorViewModel
{
    public SHA256CalculatorViewModel() : base(HashAlgorithmType.SHA256, SHA256.Create())
    {
    }
}

public sealed class SHA384CalculatorViewModel : HashCalculatorViewModel
{
    public SHA384CalculatorViewModel() : base(HashAlgorithmType.SHA384, SHA384.Create())
    {
    }
}

public sealed class SHA512CalculatorViewModel : HashCalculatorViewModel
{
    public SHA512CalculatorViewModel() : base(HashAlgorithmType.SHA512, SHA512.Create())
    {
    }
}
