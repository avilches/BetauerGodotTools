using System;
using System.Linq.Expressions;

namespace Betauer.Core;

/// <summary>
/// Cast a int to a generic enum without boxing (that means not allocating new memory during the conversion)
/// https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
///
/// Class to cast to type <see cref="T"/>
/// </summary>
/// <typeparam name="T">Target type</typeparam>
public static class CastTo<T> {
    /// <summary>
    /// Casts <see cref="TSource"/> to <see cref="T"/>.
    /// This does not cause boxing for value types.
    /// Useful in generic methods.
    /// </summary>
    /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
    public static T From<TSource>(TSource s) {
        return Cache<TSource>.CasterFunc(s);
    }

    private static class Cache<S> {
        public static readonly Func<S, T> CasterFunc = Get();

        private static Func<S, T> Get() {
            var p = Expression.Parameter(typeof(S));
            var c = Expression.ConvertChecked(p, typeof(T));
            return Expression.Lambda<Func<S, T>>(c, p).Compile();
        }
    }
}