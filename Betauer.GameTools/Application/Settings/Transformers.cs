using System;
using System.Collections.Generic;
using Betauer.Application.Screen;
using Godot;

namespace Betauer.Application.Settings {
    public interface ITransformer {
        object ToVariant(object d);
        object FromVariant(object d);
    }

    public class ResolutionTransformer : ITransformer {
        public object ToVariant(object d) => ((Resolution)d).Size;
        public object FromVariant(object d) => new Resolution((Vector2)d);
    }

    public static class Transformers {
        private static readonly Dictionary<Type, ITransformer> Registry = new Dictionary<Type, ITransformer>();

        static Transformers() {
            AddTransformer(new ResolutionTransformer(), typeof(Resolution), typeof(ScaledResolution));
        }

        public static void AddTransformer(ITransformer transformer, params Type[] types) {
            foreach (var type in types) Registry[type] = transformer;
        }

        public static object ToVariant(object value) {
            return Registry.TryGetValue(value.GetType(), out var t) ? t.ToVariant(value) : value;
        }

        public static object FromVariant(Type type, object value) {
            return Registry.TryGetValue(type, out var t) ? t.FromVariant(value) : value;
        }
    }
}