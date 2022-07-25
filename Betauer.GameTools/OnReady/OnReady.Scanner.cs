using System;
using System.Reflection;
using Betauer.DI;
using Godot;

namespace Betauer.OnReady {
    public class OnReadyScanner {
        private const BindingFlags OnReadyFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static void ScanAndInject(Node target) {
            foreach (var setter in target.GetType().GetPropertiesAndFields<OnReadyAttribute>(OnReadyFlags))
                LoadOnReadyField(target, setter);
        }

        private static void LoadOnReadyField(Node target, GetterSetter<OnReadyAttribute> getterSetter) {
            var nullable = getterSetter.Attribute.Nullable; 
            var path = getterSetter.Attribute.Path?.Trim(); 
            if (path == null) return;
            // [OnReady("path/to/node")
            // private Sprite sprite = this.GetNode<Sprite>("path/to/node");
            var node = target.GetNode(path);
            var fieldInfo = "[OnReady(\"" + path + "\")] " + getterSetter.Type.Name + " " +
                            getterSetter.Name;

            if (node == null) {
                if (nullable) return;
                throw new OnReadyFieldException(getterSetter.Name, target,
                    "Path returns a null value for field " + fieldInfo + ", class " + target.GetType().Name);
            }
            if (!getterSetter.Type.IsInstanceOfType(node)) {
                throw new OnReadyFieldException(getterSetter.Name, target,
                    "Path returns an incompatible type " + node.GetType().Name + " for field " + fieldInfo +
                    ", class " + target.GetType().Name);
            }
            getterSetter.SetValue(target, node);
        }
        
    }
}