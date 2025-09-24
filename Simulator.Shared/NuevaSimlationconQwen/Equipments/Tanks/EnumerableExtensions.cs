using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Devuelve un elemento aleatorio de la colección, o default(T) si está vacía.
    /// </summary>
    public static T RandomElement<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var list = source as IList<T> ?? source.ToList();
        if (list.Count == 0) return default(T)!;

        var index = _random.Next(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// Devuelve un elemento aleatorio de la colección, o null si está vacía (para tipos de referencia).
    /// </summary>
    public static T RandomElementOrDefault<T>(this IEnumerable<T> source) where T : class
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var list = source as IList<T> ?? source.ToList();
        return list.Count == 0 ? null! : list[_random.Next(0, list.Count)];
    }
}