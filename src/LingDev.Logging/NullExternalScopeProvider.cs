﻿using Microsoft.Extensions.Logging;

namespace LingDev.Logging;

internal sealed class NullExternalScopeProvider : IExternalScopeProvider
{
    public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

    private NullExternalScopeProvider()
    {
    }

    void IExternalScopeProvider.ForEachScope<TState>(Action<object, TState> callback, TState state)
    {
    }

    IDisposable IExternalScopeProvider.Push(object? state)
    {
        return NullScope.Instance;
    }
}
