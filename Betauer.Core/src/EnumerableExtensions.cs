using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core;

public static partial class EnumerableExtensions {
    /// <summary>
    /// Immediately executes the given action on each element in the source sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var element in source)
            action(element);
    }

    /// <summary>
    /// Immediately executes the given action on each element in the source sequence.
    /// Each element's index is used in the logic of the action.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element; the second parameter
    /// of the action represents the index of the source element.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        var index = 0;
        foreach (var element in source)
            action(element, index++);
    }

    /// <summary>
    /// Executes the given action on each element in the source sequence and returns the sequence itself.
    /// Unlike ForEach, this method can be used in the middle of a LINQ chain.
    /// </summary>
    /// <example>
    /// var numbers = new[] { 1, 2, 3, 4, 5 };
    /// var result = numbers
    ///     .Do(x => Console.WriteLine($"Before: {x}"))
    ///     .Select(x => x * 2)
    ///     .Do(x => Console.WriteLine($"After: {x}"))
    ///     .ToList();
    /// </example>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence of elements</param>
    /// <param name="action">The action to execute on each element</param>
    /// <returns>The source sequence</returns>
    public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action) {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var element in source) {
            action(element);
            yield return element;
        }
    }
}