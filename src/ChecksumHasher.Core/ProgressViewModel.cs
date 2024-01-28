using CommunityToolkit.Mvvm.ComponentModel;
using KozmoTech.CoreFx.System;
using System.Diagnostics;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

public enum HashingProgressStage
{
    NotStarted, Preparing, Calculating, Verifying, Completed, Failed,
}

/// <summary>
/// A view model that represents the progress during the computation or verification of a file’s checksums.
/// It tells you two things: what <see cref="Stage"/> it's currently at (like <see cref="HashingProgressStage.Calculating"/>)
/// and how much <see cref="Percentage"/> is done overall.
/// </summary>
/// <remarks>
/// <example><code>
/// var progress = new ProgressViewModel();
/// await using (progress.StartNewOperation())
/// {
///     progress.StageProgress.Report(HashingProgressStage.Calculating);
///     progress.PercentageProgress.Report(0.5);
///     progress.Complete();
/// }
/// </code></example>
/// </remarks>
public sealed partial class ProgressViewModel : ObservableObject, IDisposable
{
    internal ProgressViewModel()
    {
        StageProgress = new Progress<HashingProgressStage>(v => Stage = v);
        PercentageProgress = new Progress<double>(SetPercentageThrottled);
    }

    public void Dispose() => _throttleTimer?.Dispose();

    private HashingProgressStage _stage = HashingProgressStage.NotStarted;
    public HashingProgressStage Stage { get => _stage; private set => SetProperty(ref _stage, value); }

    private double _percentage = 0;
    public double Percentage { get => _percentage; private set => SetProperty(ref _percentage, value); }

    private TimeSpan _remainingTime = TimeSpan.Zero;
    public TimeSpan RemainingTime { get => _remainingTime; private set => SetProperty(ref _remainingTime, value); }

    #region Internal Progress Reporting

    internal IProgress<HashingProgressStage> StageProgress { get; }

    internal IProgress<double> PercentageProgress { get; }

    /// <summary>
    /// Start a new long running operation session.
    /// It is recommended to <see cref="IAsyncDisposable.DisposeAsync"/> the return value once the long running operation has finished (succeeded or failed).
    /// </summary>
    /// <returns>An <see cref="IAsyncDisposable"/> whose <see cref="IAsyncDisposable.DisposeAsync"/> method marks the end of the long running operation.</returns>
    internal IAsyncDisposable StartNewOperation()
    {
        _throttleTimer?.Dispose();
        Stage = HashingProgressStage.NotStarted;
        Percentage = _percentageValue = 0;
        RemainingTime = TimeSpan.MaxValue;
        _timeEstimateWatch.Restart();

        var pullingTask = StartThrottlePullingTask();
        return new AsyncScopeDefer(async () =>
        {
            _throttleTimer?.Dispose();
            _timeEstimateWatch.Stop();
            await pullingTask;
            if (Stage != HashingProgressStage.Completed)
            {
                Stage = HashingProgressStage.Failed;
            }
        });
    }

    internal void Complete()
    {
        _throttleTimer?.Dispose();
        _timeEstimateWatch.Stop();
        Stage = HashingProgressStage.Completed;
        Percentage = 1;
        RemainingTime = TimeSpan.Zero;
    }

    #endregion Internal Progress Reporting

    #region Throttling

    private static readonly TimeSpan ThrottlePeriod = TimeSpan.FromMilliseconds(200);

    private PeriodicTimer? _throttleTimer = null;
    private double _percentageValue = 0;

    private async Task StartThrottlePullingTask()
    {
#if DEBUG
        Debug.WriteLine("Throttle Pulling Thread Started");
#endif
        _throttleTimer = new(ThrottlePeriod);
        while (await _throttleTimer.WaitForNextTickAsync())
        {
            Percentage = _percentageValue;
            UpdateRemainingTime();
        }
#if DEBUG
        Debug.WriteLine("Throttle Pulling Thread Terminated");
#endif
    }

    private void SetPercentageThrottled(double value) => _percentageValue = value;

    #endregion Throttling

    #region Remaining Time Estimation

    private const double ExponentialFactor = 0.618;

    private readonly Stopwatch _timeEstimateWatch = new();
    private double _avgSpeed;

    private void UpdateRemainingTime()
    {
        try
        {
            var curSpeed = Percentage / _timeEstimateWatch.Elapsed.TotalSeconds;
            _avgSpeed = ExponentialFactor * curSpeed + (1 - ExponentialFactor) * _avgSpeed;
            RemainingTime = TimeSpan.FromSeconds((1 - Percentage) / _avgSpeed);
        }
        catch (OverflowException) // Duration is too long
        {
        }
    }

    #endregion Remaining Time Estimation

}
