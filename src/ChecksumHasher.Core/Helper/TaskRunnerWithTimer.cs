namespace KozmoTech.ZenUtility.System.Threading.Tasks;

/// <summary>
/// A task runner which will also invoke a callback method periodically as the task is running.
/// </summary>
internal sealed class TaskRunnerWithTimer
{
    public TaskRunnerWithTimer(TimeSpan interval, Action timerCallback)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), $"{nameof(interval)} must be positive");
        }

        TimerInterval = interval;
        this.timerCallback = timerCallback;
    }

    public TimeSpan TimerInterval { get; }

    public async Task RunAsync(Task work)
    {
        var (timer, timerTask) = PrepareTimer();
        try
        {
            using (timer)
            {
                await work;
            }
            await (timerTask ?? Task.CompletedTask);
        }
        catch (TaskCanceledException)
        {
            await (timerTask ?? Task.CompletedTask);
            throw;
        }
    }

    public async Task<T> RunAsync<T>(Task<T> work)
    {
        var (timer, timerTask) = PrepareTimer();
        try
        {
            T result;
            using (timer)
            {
                result = await work;
            }
            await (timerTask ?? Task.CompletedTask);
            return result;
        }
        catch (TaskCanceledException)
        {
            await (timerTask ?? Task.CompletedTask);
            throw;
        }
    }

    private (PeriodicTimer? Timer, Task? TimerTask) PrepareTimer()
    {
        if (TimerInterval > TimeSpan.Zero)
        {
            var timer = new PeriodicTimer(TimerInterval);
            return (timer, RunTimerLoopAsync(timer));
        }
        else
        {
            return (null, null);
        }
    }

    private async Task RunTimerLoopAsync(PeriodicTimer timer)
    {
        while (await timer.WaitForNextTickAsync())
        {
            timerCallback();
        }
    }

    private readonly Action timerCallback;
}
