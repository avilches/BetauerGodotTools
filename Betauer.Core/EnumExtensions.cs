using System;
using System.Linq.Expressions;

namespace Betauer {
    public static class EnumExtensions {
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
    }
}