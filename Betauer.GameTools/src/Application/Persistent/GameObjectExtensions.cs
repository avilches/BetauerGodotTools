using Godot;

namespace Betauer.Application.Persistent;

public static class GameObjectExtensions {
    private static readonly StringName GameObjectId = "__GameObjectId";
    private static readonly StringName NodeId = "__NodeId";

    public static int GetGameObjectIdFromMeta(this GodotObject o) =>
        o.GetMeta(GameObjectId).AsInt32();

    public static void SetNodeIdToMeta(this GodotObject o, GameObject gameObject) =>
        o.SetMeta(GameObjectId, gameObject.Id);

    public static bool HasMetaGameObjectId(this GodotObject o) =>
        o.HasMeta(GameObjectId);

    public static ulong GetNodeIdFromMeta(this GodotObject o) =>
        o.GetMeta(NodeId).AsUInt64();

    public static T GetNodeFromMeta<T>(this GodotObject o) where T : GodotObject =>
        (T)GodotObject.InstanceFromId(GetNodeIdFromMeta(o));

    public static void SetNodeIdToMeta(this GodotObject o, GodotObject item) =>
        o.SetMeta(NodeId, item.GetInstanceId());

    public static bool HasMetaNodeId(this GodotObject o) =>
        o.HasMeta(NodeId);
}