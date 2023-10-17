using System;
using Godot;
using Expression = System.Linq.Expressions.Expression;

namespace Betauer.Core;

public static class EnumExtensions {
    /// <summary>
    /// Cast a generic enum to int without boxing (that means not allocating new memory during the conversion)
    /// From: https://stackoverflow.com/questions/16960555/how-do-i-cast-a-generic-enum-to-int
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static int ToInt<TEnum>(this TEnum value) where TEnum : Enum {
        return CastEnumToIntFuncHolder<TEnum>.CastEnumToInt(value);
    }

    private static class CastEnumToIntFuncHolder<T> where T : Enum {
        internal static readonly Func<T, int> CastEnumToInt = GenerateCastEnumToIntFunc<T>();

        private static Func<TEnum, int> GenerateCastEnumToIntFunc<TEnum>() where TEnum : Enum {
            var inputParameter = Expression.Parameter(typeof(TEnum));
            var body = Expression.Convert(inputParameter, typeof(int)); // means: (int)input;
            var lambda = Expression.Lambda<Func<TEnum, int>>(body, inputParameter);
            return lambda.Compile();
        }
    }
    
    /// <summary>
    /// Cast a int to a generic enum without boxing (that means not allocating new memory during the conversion)
    /// https://stackoverflow.com/questions/1189144/c-sharp-non-boxing-conversion-of-generic-enum-to-int
    public static TEnum ToEnum<TEnum>(this int s) where TEnum : Enum {
        return Cache<TEnum, int>.CastIntToEnum(s);
    }

    public static TEnum ToEnum<TEnum>(this long s) where TEnum : Enum {
        return Cache<TEnum, long>.CastIntToEnum(s);
    }

    private static class Cache<TEnum, S> {
        internal static readonly Func<S, TEnum> CastIntToEnum = Get();

        private static Func<S, TEnum> Get() {
            var p = Expression.Parameter(typeof(S));
            var c = Expression.ConvertChecked(p, typeof(TEnum));
            return Expression.Lambda<Func<S, TEnum>>(c, p).Compile();
        }
    }
}