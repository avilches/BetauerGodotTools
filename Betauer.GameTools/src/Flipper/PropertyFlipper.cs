using Godot;

namespace Betauer.Flipper;

public class PropertyFlipper<[MustBeVariant] T> : Flipper {
    private readonly Node _node;
    private readonly string _property;
    private readonly T _valueWhenRight;
    private readonly T _valueWhenLeft;

    public PropertyFlipper(Node node, string property, T valueWhenRight, T valueWhenLeft) {
        _node = node;
        _property = property;
        _valueWhenRight = valueWhenRight;
        _valueWhenLeft = valueWhenLeft;
    }

    public override bool LoadIsFacingRight() {
        return _node.GetIndexed(_property).As<T>()!.Equals(_valueWhenRight);
    }

    public override void SetFacingRight(bool right) {
        _node.SetIndexed(_property, Variant.From(right ? _valueWhenRight : _valueWhenLeft));
    }
}