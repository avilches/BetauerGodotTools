using System;
using System.Reflection;
using Betauer.DI;
using Godot;

namespace Betauer {
    public class OnReadyScanner {
        private const BindingFlags OnReadyFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static void ScanAndInject(Node target) {
            var fields = target.GetType().GetFields(OnReadyFlags);
            foreach (var field in fields) {
                if (Attribute.GetCustomAttribute(field, typeof(OnReadyAttribute), false) is 
                    OnReadyAttribute onReady) {
                    LoadOnReadyField(target, onReady, new Setter(field));
                }
            }

            var properties = target.GetType().GetProperties(OnReadyFlags);
            foreach (var property in properties) {
                if (Attribute.GetCustomAttribute(property,
                        typeof(OnReadyAttribute), false) is OnReadyAttribute onReady) {
                    LoadOnReadyField(target, onReady, new Setter(property));
                }
            }
        }

        private static void LoadOnReadyField(Node target, OnReadyAttribute onReady, Setter setter) {
            if (onReady.Path == null) return;
            var path = onReady.Path.Trim();
            // [OnReady("path/to/node")
            // private Sprite sprite = this.GetNode<Sprite>("path/to/node");
            var node = target.GetNode(path);
            var fieldInfo = "[OnReady(\"" + path + "\")] " + setter.Type.Name + " " +
                            setter.Name;

            if (node == null) {
                if (onReady.Nullable) return;
                throw new OnReadyFieldException(setter.Name, target,
                    "Path returns a null value for field " + fieldInfo + ", class " + target.GetType().Name);
            }
            if (!setter.Type.IsInstanceOfType(node)) {
                throw new OnReadyFieldException(setter.Name, target,
                    "Path returns an incompatible type " + node.GetType().Name + " for field " + fieldInfo +
                    ", class " + target.GetType().Name);
            }
            setter.SetValue(target, node);
        }
        
    }
}