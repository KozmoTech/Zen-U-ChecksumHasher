using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;

namespace KozmoTech.ZenUtility.ChecksumHasher;

public abstract class HashCalculatorViewModel : ObservableObject
{
    protected HashCalculatorViewModel(HashAlgorithmType algorithm, HashAlgorithm hasher)
    {
        Algorithm = algorithm;
        this.hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
    }

    public HashAlgorithmType Algorithm { get; }

    private readonly HashAlgorithm hasher;
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
