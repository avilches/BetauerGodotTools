using System;
using Godot;

namespace Betauer.Core;

public static class Predicates {
    public static Func<bool> IsValid(GodotObject o) => () => GodotObject.IsInstanceValid(o);

    public static Func<bool> IsInvalid(GodotObject o) => () => !GodotObject.IsInstanceValid(o);

    public static Func<bool> IsInsideTree(Node n) => n.IsInsideTree;

    public static Func<bool> IsOutsideTree(Node n) => () => !n.IsInsideTree();
}