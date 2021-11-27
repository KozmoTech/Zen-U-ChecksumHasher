using CommunityToolkit.Mvvm.ComponentModel;
using KozmoTech.ZenUtility.System;
using KozmoTech.ZenUtility.System.IO;
using System.Diagnostics;
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

    public double ComputeProgress
    {
        get => progress;
        set => SetProperty(ref progress, value);
    }

    private readonly DisposableHelper disposable;
    private readonly HashAlgorithm hasher;
    private double progress;
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
