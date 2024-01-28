using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public enum HashAlgorithmType
{
    MD5, SHA1, SHA256, SHA384, SHA512,
}

public sealed partial class HashCalculatorViewModel : ObservableObject, IDisposable
{
    internal HashCalculatorViewModel(HashAlgorithmType algorithm) =>
        _hasher = (Algorithm = algorithm) switch
        {
            HashAlgorithmType.MD5 => MD5.Create(),
            HashAlgorithmType.SHA1 => SHA1.Create(),
            HashAlgorithmType.SHA256 => SHA256.Create(),
            HashAlgorithmType.SHA384 => SHA384.Create(),
            HashAlgorithmType.SHA512 => SHA512.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), $"{algorithm} is not supported"),
        };

    public void Dispose() => _hasher.Dispose();

    public HashAlgorithmType Algorithm { get; }

    [ObservableProperty]
    private bool _isEnabled = true;

    private byte[]? _hashCode = null;
    public byte[]? HashCode
    {
        get => _hashCode;
        private set
        {
            if (SetProperty(ref _hashCode, value))
            {
                OnPropertyChanged(nameof(HashCodeString));
            }
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HashCodeString))]
    private HashStringFormat _hashCodeFormat = HashStringFormat.LowerCaseNoDash;

    public string? HashCodeString => HashCode?.ToString(HashCodeFormat);

    internal void Reset()
    {
        HashCode = null;
        _hasher.Initialize();
    }

    internal void AppendBlock(ArraySegment<byte> block) => _hasher.TransformBlock(block.Array!, block.Offset, block.Count, null, 0);

    internal void Finish()
    {
        _hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        HashCode = _hasher.Hash;
    }

    private readonly HashAlgorithm _hasher;
}

public enum HashStringFormat
{
    UpperCaseNoDash, LowerCaseNoDash,
}

internal static class HashCodeToStringExtensions
{
    internal static string ToString(this byte[] @this, HashStringFormat format) =>
        format switch
        {
            HashStringFormat.LowerCaseNoDash => string.Join("", from b in @this select b.ToString("x2")),
            HashStringFormat.UpperCaseNoDash => string.Join("", from b in @this select b.ToString("X2")),
            _ => BitConverter.ToString(@this),
        };
}
