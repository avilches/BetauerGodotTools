using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Core;

public static class Predicates {
    public static Func<bool> IsValid(Object o) => () => Object.IsInstanceValid(o);

    public static Func<bool> IsInvalid(Object o) => () => !Object.IsInstanceValid(o);

    public static Func<bool> IsInsideTree(Node n) => n.IsInsideTree;

    public static Func<bool> IsOutsideTree(Node n) => () => !n.IsInsideTree();
}