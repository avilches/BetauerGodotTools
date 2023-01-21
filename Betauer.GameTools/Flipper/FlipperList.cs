using System;
using System.Linq;
using Godot;

namespace Betauer.Flipper;

public class FlipperList : Flipper {
    private IFlipper[] _flippers = Array.Empty<IFlipper>();

    public FlipperList Property<[MustBeVariant] TProperty>(Node node, string property, TProperty valueWhenRight, TProperty valueWhenLeft) =>
        Add(new PropertyFlipper<TProperty>(node, property, valueWhenRight, valueWhenLeft));

    public FlipperList Sprite2DFlipH(Sprite2D sprite) =>
        Add(new Sprite2DFlipH(sprite));

    public FlipperList ScaleX(Node2D node2D) =>
        Add(new FlipScaleX(node2D));

    public FlipperList Add(IFlipper flipper) {
        _flippers = _flippers.Concat(new[] { flipper }).ToArray();
        return this;
    }

    public FlipperList SetFlippers(params IFlipper[] flippers) {
        _flippers = flippers;
        return this;
    }

    public override bool LoadIsFacingRight() => _flippers[0].IsFacingRight;

    public override void SetFacingRight(bool right) => 
        Array.ForEach(_flippers, flipper => flipper.IsFacingRight = right);
}