namespace KozmoTech.CoreFx.System;

/// <summary>
/// A scope guard which is similar to the Golang's <seealso href="https://gobyexample.com/defer">"defer statement"</seealso>.
/// </summary>
/// <example>
/// <code>
/// using var defer = new ScopeDefer(() => ...);
/// </code>
/// </example>
public sealed class ScopeDefer : IDisposable, IDisposableManaged
{
    public ScopeDefer(Action disposing)
    {
        disposable = new(this);
        disposingAction = disposing ?? throw new ArgumentNullException(nameof(disposing));
    }

    public void Dispose() => disposable.DoDispose();

    public void DisposeManagedResources() => disposingAction();

    private readonly Action disposingAction;
    private readonly DisposableHelper disposable;
}
