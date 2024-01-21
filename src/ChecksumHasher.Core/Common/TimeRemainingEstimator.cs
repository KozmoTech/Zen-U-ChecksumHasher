using System.Diagnostics;

namespace KozmoTech.ZenUtility.ChecksumHasher.Core;

/// <summary>
/// An estimator which will use the exponential weighted algorithm to calculate the remaining time.
/// </summary>
internal sealed class TimeRemainingEstimator
{
    public TimeSpan? TimeRemaining => totalEstimatedTime * (1 - Progress);

    public void Stop()
    {
        stopwatch.Stop();
        totalEstimatedTime = null;
    }

    public void Start()
    {
        progress = 0;
        totalEstimatedTime = null;
        stopwatch.Restart();
        initialSamples.Clear();
    }

    public double Progress
    {
        get => progress;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(Progress), $"{nameof(Progress)} must be a number between [0, 1]");
            }

            progress = value;
            try
            {
                AppendSample(stopwatch.Elapsed / value);
            }
            catch (OverflowException)
            {
            }
        }
    }

    private void AppendSample(TimeSpan sample)
    {
        if (initialSamples.Count < InitialSamplingCount)
        {
            initialSamples.Add(sample);
            return;
        }
        if (totalEstimatedTime is null)
        {
            var min = initialSamples.Min();
            var max = initialSamples.Max();
            initialSamples.Remove(min);
            initialSamples.Remove(max);
            totalEstimatedTime = initialSamples.First();
            foreach (var initialSample in initialSamples.Skip(1))
            {
                UpdateTotalTime(initialSample);
            }
        }
        UpdateTotalTime(sample);
    }

    private void UpdateTotalTime(TimeSpan sample) => totalEstimatedTime = sample * ExponentialFactor + totalEstimatedTime * (1 - ExponentialFactor);

    private readonly List<TimeSpan> initialSamples = new();
    private readonly Stopwatch stopwatch = new();
    private double progress = 0;
    private TimeSpan? totalEstimatedTime = null;

    private const int InitialSamplingCount = 5;
    private const double ExponentialFactor = 0.5;
}
