using CommunityToolkit.Mvvm.ComponentModel;
using KozmoTech.ZenUtility.System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KozmoTech.ZenUtility.ChecksumHasher;

public sealed partial class FileHasherViewModel : ObservableObject, IDisposable, IDisposableNullifyFields
{
    public FileHasherViewModel()
    {
        disposable = new(this);
        Hashers = new(hashers);
    }

    public void Dispose() => disposable.DoDispose(hashers.ToArray());
    void IDisposableNullifyFields.DisposeNullifyFields() => hashers.Clear();

    public ReadOnlyObservableCollection<HashCalculatorViewModel> Hashers { get; }

    public bool UseMD5
    {
        get => GetHasher(HashAlgorithmType.MD5) is not null;
        set => UseHasher(HashAlgorithmType.MD5, value);
    }

    public bool UseSHA1
    {
        get => GetHasher(HashAlgorithmType.SHA1) is not null;
        set => UseHasher(HashAlgorithmType.SHA1, value);
    }

    public bool UseSHA256
    {
        get => GetHasher(HashAlgorithmType.SHA256) is not null;
        set => UseHasher(HashAlgorithmType.SHA256, value);
    }

    public bool UseSHA384
    {
        get => GetHasher(HashAlgorithmType.SHA384) is not null;
        set => UseHasher(HashAlgorithmType.SHA384, value);
    }

    public bool UseSHA512
    {
        get => GetHasher(HashAlgorithmType.SHA512) is not null;
        set => UseHasher(HashAlgorithmType.SHA512, value);
    }

    /// <summary>
    /// Get a hasher in <see cref="Hashers"/> whose <see cref="HashCalculatorViewModel.Algorithm"/> is <paramref name="algorithm"/>.
    /// </summary>
    /// <param name="algorithm">The corresponding <see cref="HashAlgorithmType"/> to check.</param>
    /// <returns>An instance of <see cref="HashCalculatorViewModel"/> if exists, or <c>null</c> if not exists.</returns>
    private HashCalculatorViewModel? GetHasher(HashAlgorithmType algorithm) => Hashers.FirstOrDefault(x => x.Algorithm == algorithm);

    /// <summary>
    /// Ensure a <see cref="HashCalculatorViewModel"/> whose <see cref="HashCalculatorViewModel.Algorithm"/> is <paramref name="algorithm"/>, must exist or must not exist in
    /// <see cref="Hashers"/>, depends on <paramref name="value"/> parameter.
    /// </summary>
    /// <param name="algorithm">The corresponding <see cref="HashAlgorithmType"/> to ensure.</param>
    /// <param name="value"><c>true</c> means must exist, and <c>false</c> means must not exist.</param>
    /// <param name="propertyName">The observable property name to raise property changing events.</param>
    private void UseHasher(HashAlgorithmType algorithm, bool value, [CallerMemberName] string? propertyName = null)
    {
        var currHasher = GetHasher(algorithm);
        SetProperty(currHasher is not null, value, v =>
        {
            if (v)
            {
                hashers.Add(CreateHasher(algorithm));
            }
            else
            {
                Debug.Assert(currHasher is not null);
                using (currHasher)
                {
                    hashers.Remove(currHasher);
                }
            }
        }, propertyName);
    }

    private static HashCalculatorViewModel CreateHasher(HashAlgorithmType algorithm)
    {
        HashCalculatorViewModel hasher = algorithm switch
        {
            HashAlgorithmType.MD5 => new MD5CalculatorViewModel(),
            HashAlgorithmType.SHA1 => new SHA1CalculatorViewModel(),
            HashAlgorithmType.SHA256 => new SHA256CalculatorViewModel(),
            HashAlgorithmType.SHA384 => new SHA384CalculatorViewModel(),
            HashAlgorithmType.SHA512 => new SHA512CalculatorViewModel(),
            _ => throw new NotSupportedException($"algorithm {algorithm} is not supported"),
        };

        Debug.Assert(hasher is not null && hasher.Algorithm == algorithm);
        return hasher;
    }

    private readonly DisposableHelper disposable;
    private readonly ObservableCollection<HashCalculatorViewModel> hashers = new()
    {
        CreateHasher(HashAlgorithmType.MD5),
        CreateHasher(HashAlgorithmType.SHA256),
    };
}
