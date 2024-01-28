namespace KozmoTech.CoreFx.System;

/// <summary>
/// A scope guard which is similar to the Golang's <seealso href="https://gobyexample.com/defer">"defer statement"</seealso>.
/// </summary>
/// <remarks>
/// <example><code>
/// using var defer = new ScopeDefer(() => ...);
/// </code></example>
/// </remarks>
public sealed class ScopeDefer : IDisposable
{
    public ScopeDefer(Action disposing) => _disposingAction = disposing ?? throw new ArgumentNullException(nameof(disposing));

    public void Dispose() => _disposingAction();

    private readonly Action _disposingAction;
}

/// <summary>
/// A scope guard which is similar to the Golang's <seealso href="https://gobyexample.com/defer">"defer statement"</seealso>, but asynchronously.
/// </summary>
/// <remarks>
/// <example><code>
/// await using var defer = new ScopeDefer(async () => ...);
/// </code></example>
/// </remarks>
public sealed class AsyncScopeDefer : IAsyncDisposable
{
    public AsyncScopeDefer(Func<ValueTask> disposing) => _disposingTask = disposing ?? throw new ArgumentNullException(nameof(disposing));

    public ValueTask DisposeAsync() => _disposingTask();

    private readonly Func<ValueTask> _disposingTask;
}
