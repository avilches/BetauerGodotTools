
using Godot;

namespace Betauer {
    public static class ObjectExtensions {

        public static string ToStringSafe(this Object o) {
            var extra = o is SceneTreeTween sceneTreeTween ? (sceneTreeTween.IsValid() ? "" : " (tween invalid)") : "";
            return Object.IsInstanceValid(o) ?
                $"{o.GetType().Name}@{o.GetHashCode():x8} ({o.GetInstanceId()}){extra}" :
                $"{o.GetType().Name}@{o.GetHashCode():x8} (disposed)"; // "InstanceId:" + o.GetInstanceId()+ " Ptr:"+o.NativeInstance;
        }
    }
}