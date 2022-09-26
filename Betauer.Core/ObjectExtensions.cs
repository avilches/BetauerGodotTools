
using Godot;

namespace Betauer {
    public static class ObjectExtensions {

        public static Object FreeDeferred(this Object o) {
            o.CallDeferred("free");
            return o;
        }

        public static string ToStringSafe(this Object o) {
            var typeName = o.GetType().Name;
            var hashCode = o.GetHashCode();
            if (!Object.IsInstanceValid(o)) {
                return $"{typeName} @{hashCode:x8} (disposed)";
            }
            return o switch {
                SceneTreeTween sceneTreeTween => 
                    $"{typeName} @{hashCode:x8} ({o.GetInstanceId()}){(sceneTreeTween.IsValid() ? "" : " (invalid)")}",
                Node node => 
                    $"{typeName} \"{node.Name}\" @{hashCode:x8} ({o.GetInstanceId()})",
                _ => 
                    $"{typeName}@{hashCode:x8} ({o.GetInstanceId()})"
            };
        }
    }
}