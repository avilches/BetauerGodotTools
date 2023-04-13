
using Godot;

namespace Betauer.Core; 

public static class ObjectExtensions {

    public static bool IsInstanceValid(this GodotObject o) => GodotObject.IsInstanceValid(o);
        
    public static bool IsInstanceInvalid(this GodotObject o) => !GodotObject.IsInstanceValid(o);

    public static GodotObject FreeDeferred(this GodotObject o) {
        o.CallDeferred("free");
        return o;
    }

    public static string ToStringSafe(this GodotObject o) {
        var typeName = o.GetType().Name;
        var hashCode = o.GetHashCode();
        if (!GodotObject.IsInstanceValid(o)) {
            return $"{typeName} @{hashCode:x8} (disposed)";
        }
        return o switch {
            Tween sceneTreeTween => 
                $"{typeName} @{hashCode:x8} ({o.GetInstanceId()}){(sceneTreeTween.IsValid() ? "" : " (invalid)")}",
            Node node => 
                $"{typeName} \"{node.Name}\" @{hashCode:x8} ({o.GetInstanceId()})",
            _ => 
                $"{typeName}@{hashCode:x8} ({o.GetInstanceId()})"
        };
    }
}