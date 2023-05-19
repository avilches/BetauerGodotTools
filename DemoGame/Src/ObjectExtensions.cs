using Godot;

namespace Veronenger;

public static partial class ObjectExtensions {
    private static readonly StringName CollisionNodeId = new("CollisionNodeId");

    public static ulong GetCollisionNodeId(this GodotObject o) =>
        o.GetMeta(CollisionNodeId).AsUInt64();

    public static T GetCollisionNode<T>(this GodotObject o) where T : GodotObject =>
        (T)GodotObject.InstanceFromId(GetCollisionNodeId(o));

    public static void SetCollisionNode(this GodotObject o, GodotObject item) =>
        o.SetMeta(CollisionNodeId, item.GetInstanceId());

    public static bool HasCollisionNode(this GodotObject o) =>
        o.HasMeta(CollisionNodeId);
}