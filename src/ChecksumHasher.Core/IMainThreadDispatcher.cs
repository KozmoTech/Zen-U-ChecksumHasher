﻿namespace KozmoTech.ZenUtility.ChecksumHasher;

public interface IMainThreadDispatcher
{
    void Dispatch(Action worker);
}

public sealed class NoopDispatcher : IMainThreadDispatcher
{
    public void Dispatch(Action worker) => worker();

    public static IMainThreadDispatcher Instance { get; } = new NoopDispatcher();
}
