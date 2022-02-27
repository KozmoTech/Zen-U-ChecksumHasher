using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KozmoTech.CoreFx.System;
using KozmoTech.CoreFx.System.Algorithm;
using KozmoTech.CoreFx.System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public sealed partial class FileHasherViewModel : ObservableObject, IDisposable
{
    public FileHasherViewModel() => disposable = new(this);

    public void Dispose() => disposable.DoDispose(Hashers.ToArray());

    [ObservableProperty]
    private FileInfoViewModel? fileInfo = null;

    /// <summary>
    /// Get a collection of all supported hashers. Use <see cref="HashCalculatorViewModel.IsEnabled"/> to control whether
    /// a specific hasher should participate in the calculation.
    /// </summary>
    public ReadOnlyCollection<HashCalculatorViewModel> Hashers { get; } = new List<HashCalculatorViewModel>
    {
        new MD5CalculatorViewModel(),
        new SHA1CalculatorViewModel { IsEnabled = false },
        new SHA256CalculatorViewModel(),
        new SHA384CalculatorViewModel { IsEnabled = false },
        new SHA512CalculatorViewModel(),
    }.AsReadOnly();

    public double OverallProgress => Hashers.Average(x => x.ComputeProgress ?? 1);

    public TimeSpan TimeRemaining => timeRemaining.TimeRemaining ?? TimeSpan.Zero;

    [ICommand]
    internal async Task ComputeAllHashesAsync(CancellationToken cancellation)
    {
        if (FileInfo is null)
        {
            return;
        }

        var hashers = (from h in Hashers where h.IsEnabled select h).ToArray();
        if (hashers.Length == 0)
        {
            return;
        }

        OnPropertyChanged(nameof(OverallProgress));
        timeRemaining.Start();
        OnPropertyChanged(nameof(TimeRemaining));
        using var defer = new ScopeDefer(() =>
        {
            timeRemaining.Stop();
            OnPropertyChanged(nameof(OverallProgress));
            OnPropertyChanged(nameof(TimeRemaining));
        });

        var runner = new TaskRunnerWithTimer(ProgressUpdateInterval, Runner_TimerCallback);
        await runner.RunAsync(Task.WhenAll(from h in hashers select h.ComputeHashAsync(FileInfo, cancellation)));

        // TODO: Opt the progress timer out of this class, and treat null TotalLength as indetermine
        void Runner_TimerCallback()
        {
            OnPropertyChanged(nameof(OverallProgress));
            timeRemaining.Progress = OverallProgress;
            OnPropertyChanged(nameof(TimeRemaining));
        }
    }

    private readonly DisposableHelper disposable;
    private readonly TimeRemainingEstimator timeRemaining = new();

    private static readonly TimeSpan ProgressUpdateInterval = TimeSpan.FromSeconds(1);
}
