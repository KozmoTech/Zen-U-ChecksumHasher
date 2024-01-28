﻿using CommunityToolkit.Diagnostics;

namespace KozmoTech.CoreFx.System;

public static class CollectionExtension
{
    public static void ForEach<T>(this IReadOnlyCollection<T> @this, Action<T> callback)
    {
        Guard.IsNotNull(@this, nameof(@this));
        Guard.IsNotNull(callback, nameof(callback));

        foreach (var item in @this)
        {
            callback(item);
        }
    }

    public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> collection)
    {
        Guard.IsNotNull(@this, nameof(@this));
        Guard.IsNotNull(collection, nameof(collection));

        foreach (var item in collection)
        {
            @this.Add(item);
        }
    }

    public static void ReplaceWith<T>(this ICollection<T> @this, IEnumerable<T> collection)
    {
        Guard.IsNotNull(@this, nameof(@this));
        Guard.IsNotNull(collection, nameof(collection));

        @this.Clear();
        @this.AddRange(collection);
    }
}
