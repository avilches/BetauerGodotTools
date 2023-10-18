using System;
using Godot;
using Expression = System.Linq.Expressions.Expression;

namespace Betauer.Core;

public static class EnumExtensions {
    public static int ToInt<TEnum>(this TEnum value) where TEnum : Enum {
        return CastTo<int>.From(value);
    }

    public static TEnum ToEnum<TEnum>(this IConvertible value) where TEnum : Enum {
        return CastTo<TEnum>.From(value);
    }
}