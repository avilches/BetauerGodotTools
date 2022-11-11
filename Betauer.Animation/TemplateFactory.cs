using System;
using System.Collections.Generic;

namespace Betauer.Animation {
    public abstract class TemplateFactory {
        public readonly string? Category;
        public readonly string Name;

        protected TemplateFactory(string? category, string name) {
            Category = category;
            Name = name;
        }
    }

    public class TemplateFactory<T> : TemplateFactory where T : class {
        private readonly Func<T> _factory0P;
        private T? _cached;

        public TemplateFactory(string name, Func<T> factory0P) : this(null, name, factory0P) {
        }

        public TemplateFactory(string? category, string name, Func<T> factory0P) : base(category, name) {
            _factory0P = factory0P;
        }

        public T Get() => _cached ??= _factory0P();
    }

    public class TemplateFactory<TParam, T> : TemplateFactory<T> where T : class {
        private readonly Func<TParam, T> _factory1P;
        private Dictionary<TParam, T>? _cached1P;

        public TemplateFactory(string name, Func<T> factory, Func<TParam, T> factory1P) : this(null, name, factory, factory1P) {
        }

        public TemplateFactory(string? category, string name, 
            Func<T> factory0P, Func<TParam, T> factory1P) : base(category, name, factory0P) {
            _factory1P = factory1P;
        }

        public T Get(TParam p1) {
            _cached1P ??= new Dictionary<TParam, T>();
            if (!_cached1P.TryGetValue(p1, out T template)) {
                template = _cached1P[p1] = _factory1P(p1);
            }
            return template;
        }
    }

    public class TemplateFactory<TParam1, TParam2, T> : TemplateFactory<TParam1, T> where T : class {
        private readonly Func<TParam1, TParam2, T> _factory2P;
        private Dictionary<(TParam1, TParam2), T>? _cached2P;

        public TemplateFactory(string name, Func<T> factory, Func<TParam1, T> factory1P, Func<TParam1, TParam2, T> factory2P) : this(null, name, factory, factory1P, factory2P) {
        }

        public TemplateFactory(string? category, string name, 
            Func<T> factory0P, Func<TParam1, T> factory1P, Func<TParam1, TParam2, T> factory2P) : base(category, name, factory0P, factory1P) {
            _factory2P = factory2P;
        }

        public T Get(TParam1 p1, TParam2 p2) {
            _cached2P ??= new Dictionary<(TParam1, TParam2), T>();
            if (!_cached2P.TryGetValue((p1, p2), out T template)) {
                template = _cached2P[(p1, p2)] = _factory2P(p1, p2);;
            }
            return template;
        }
    }
}