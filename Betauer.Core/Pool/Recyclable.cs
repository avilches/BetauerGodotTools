using System;
using Object = Godot.Object;

namespace Betauer.Core.Pool {
    public interface IRecyclable {
        string GetToken();
        int GetUsages();
        public void SetPool(IObjectPool pool);
        public bool ReturnToPool();
    }

    public abstract class Recyclable : IRecyclable {
        private IObjectPool? _pool;
        private string _token;
        private int _usages;
        public string GetToken() => _token;
        public int GetUsages() => _usages;

        public bool ReturnToPool() {
            return _pool?.Return(this) ?? true;
        }

        public void SetPool(IObjectPool? pool) {
            _token = Guid.NewGuid().ToString();
            _usages++;
            _pool = pool;
        }
    }

    public abstract partial class GodotObjectRecyclable : Object, IRecyclable {
        private IObjectPool? _pool;
        private string _token;
        private int _usages;
        public string GetToken() => _token;
        public int GetUsages() => _usages;

        public bool ReturnToPool() {
            return _pool?.Return(this) ?? true;
        }

        public void SetPool(IObjectPool? pool) {
            _token = Guid.NewGuid().ToString();
            _usages++;
            _pool = pool;
        }
    }


}