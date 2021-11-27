using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KozmoTech.ZenUtility.System;

internal interface IDisposableManaged
{
    void DisposeManagedResources();
}

internal interface IDisposableUnmanaged
{
    void DisposeUnmanagedResources();
}

internal interface IDisposableNullifyFields
{
    void DisposeNullifyFields();
}

internal struct DisposableHelper
{
    public DisposableHelper(IDisposable @this) => this.@this = @this ?? throw new ArgumentNullException(nameof(@this));

    /// <summary>
    /// Make sure the user class always calls this method in its <see cref="IDisposable.Dispose"/> method.
    /// </summary>
    /// <param name="subResources">The inner <see cref="IDisposable"/> resources which can be conveniently disposed here.</param>
    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "By design")]
    public void DoDispose(params IDisposable[] subResources)
    {
        Dispose(disposing: true, subResources);
        GC.SuppressFinalize(@this);
    }

    /// <summary>
    /// Make sure the user class calls this method when it contains unmanaged resources.
    /// </summary>
    public void DoFinalize()
    {
        Debug.Assert(@this is IDisposableUnmanaged);
        Dispose(disposing: false);
    }

    private void Dispose(bool disposing, IDisposable[]? subResources = null)
    {
        if (!disposed)
        {
            if (disposing)
            {
                if (@this is IDisposableManaged managedDisposable)
                {
                    managedDisposable.DisposeManagedResources();
                }
                if (subResources is not null)
                {
                    foreach (var resource in subResources)
                    {
                        resource.Dispose();
                    }
                }
            }

            if (@this is IDisposableUnmanaged unmanagedDisposable)
            {
                unmanagedDisposable.DisposeUnmanagedResources();
            }

            if (@this is IDisposableNullifyFields nullifyFields)
            {
                nullifyFields.DisposeNullifyFields();
            }

            disposed = true;
        }
    }

    private readonly IDisposable @this;
    private bool disposed = false;
}
