using System;
using Godot;

namespace Betauer {
    public static class NodeAction {
        private const string ProxyName = "__ProxyNodeAction__";
        public static T GetProxy<T>(Node owner) where T : Node {
            T proxy = owner.GetNodeOrNull<T>(ProxyName);
            if (proxy == null) {
                proxy = Activator.CreateInstance<T>();
                proxy.Name = ProxyName;
                owner.AddChild(proxy);
            }
            return proxy;
        }
    }
}