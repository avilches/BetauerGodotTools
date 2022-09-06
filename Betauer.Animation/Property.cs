using System;
using Betauer.Animation.Tween;
using Godot;

namespace Betauer.Animation {
    public static class Property {
        public static readonly ControlOrNode2DIndexedProperty<Color> Modulate =
            new ControlOrNode2DIndexedProperty<Color>("modulate", "modulate");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateR =
            new ControlOrNode2DIndexedProperty<float>("modulate:r", "modulate:r");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateG =
            new ControlOrNode2DIndexedProperty<float>("modulate:g", "modulate:g");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateB =
            new ControlOrNode2DIndexedProperty<float>("modulate:b", "modulate:b");

        public static readonly IIndexedProperty<float> Opacity =
            new ControlOrNode2DIndexedProperty<float>("modulate:a", "modulate:a");

        /*
         * "transform.origin" matrix is used to change the position instead of "position:x" because it's safe to use
         * along with the SkewX/SkewY properties, which they use the transform.origin too.
         */
        public static readonly ControlOrNode2DIndexedProperty<Vector2> Position2D =
            new ControlOrNode2DIndexedProperty<Vector2>("transform:origin", "rect_position");

        public static readonly ControlOrNode2DIndexedProperty<float> PositionX =
            new ControlOrNode2DIndexedProperty<float>("transform:origin:x", "rect_position:x");

        public static readonly ControlOrNode2DIndexedProperty<float> PositionY =
            new ControlOrNode2DIndexedProperty<float>("transform:origin:y", "rect_position:y");

        // To avoid NPE, put these PercentPositionX, PercentPositionY and PositionBySize2D objectes
        // just before the PositionX, PositionY and PositionBySize2D
        public static readonly PositionBySizeX PositionBySizeX = new PositionBySizeX(PositionX);
        public static readonly PositionBySizeY PositionBySizeY = new PositionBySizeY(PositionY);
        public static readonly PositionBySize2D PositionBySize2D = new PositionBySize2D(Position2D);

        public static readonly ControlOrNode2DIndexedProperty<Vector2> Scale2D =
            new ControlOrNode2DIndexedProperty<Vector2>("scale", "rect_scale");

        public static readonly ControlOrNode2DIndexedProperty<float> Scale2Dx =
            new ControlOrNode2DIndexedProperty<float>("scale:x", "rect_scale:x");

        public static readonly ControlOrNode2DIndexedProperty<float> Scale2Dy =
            new ControlOrNode2DIndexedProperty<float>("scale:y", "rect_scale:y");

        public static readonly Scale2DProperty Scale2DByCallback = new Scale2DProperty();
        public static readonly ScaleXProperty Scale2DxByCallback = new ScaleXProperty();
        public static readonly ScaleYProperty Scale2DyByCallback = new ScaleYProperty();
        public static readonly ScaleZProperty Scale3DzByCallback = new ScaleZProperty();

        /**
         * It doesn't work combined with Scale or Position (with transform)
         */
        public static readonly ControlOrNode2DIndexedProperty<float> Rotate2D =
            new ControlOrNode2DIndexedProperty<float>("rotation_degrees", "rect_rotation");

        public static readonly Rotate2DProperty Rotate2DByCallback = new Rotate2DProperty();

        public static readonly ControlOrNode2DIndexedProperty<float> Skew2DX =
            new ControlOrNode2DIndexedProperty<float>("transform:y:x", null);

        public static readonly ControlOrNode2DIndexedProperty<float> Skew2DY =
            new ControlOrNode2DIndexedProperty<float>("transform:x:y", null);
    }

    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public TProperty GetValue(Node node);
        public void SetValue(AnimationContext<TProperty> context);
        public bool IsCompatibleWith(Node node);
    }

    public interface IIndexedProperty<TProperty> : IProperty<TProperty> {
        public NodePath GetIndexedPropertyName(Node node);
        public void SetValue(Node target, TProperty value);
    }


    public class CallbackProperty<TProperty> : IProperty<TProperty> {
        private readonly Action<TProperty> _action;

        public CallbackProperty(Action<TProperty> action) {
            _action = action;
        }

        public TProperty GetValue(Node node) {
            return default;
        }

        public void SetValue(AnimationContext<TProperty> context) {
            _action.Invoke(context.Value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public override string ToString() {
            return $"CallbackProperty<{typeof(TProperty).Name}>";
        }
    }

    public class NodeCallbackProperty<TProperty> : IProperty<TProperty> {
        private readonly Action<Node, TProperty> _action;

        public NodeCallbackProperty(Action<Node, TProperty> action) {
            _action = action;
        }

        public TProperty GetValue(Node node) {
            return default;
        }

        public void SetValue(AnimationContext<TProperty> context) {
            _action.Invoke(context.Target, context.Value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public override string ToString() {
            return $"CallbackProperty<Node, {typeof(TProperty).Name}>";
        }
    }

    public abstract class Property<TProperty> : IProperty<TProperty> {
        public void SetValue(AnimationContext<TProperty> context) {
            if (IsCompatibleWith(context.Target)) {
                SetValue(context.Target, context.Value);
            }
        }

        public abstract void SetValue(Node node, TProperty value);
        public abstract TProperty GetValue(Node node);
        public abstract bool IsCompatibleWith(Node node);
    }


    public class Rotate2DProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.RotationDegrees,
                Control control => control.RectRotation,
                _ => throw new Exception($"Not rotation property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D node2D) node2D.RotationDegrees = value;
            else if (node is Control control) control.RectRotation = value;
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D;
        }

        public override string ToString() {
            return "Rotate2DProperty<float>(node2D:\"RotationDegrees\", control:\"RectRotation\")";
        }
    }

    public class Scale3DProperty : Property<Vector3> {
        public override Vector3 GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale,
                _ => throw new Exception($"Not Scale3D property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, Vector3 value) {
            if (node is Spatial spatial) spatial.Scale = value;
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }

        public override string ToString() {
            return "Scale3DProperty<Vector3>(spatial: \"Scale\")";
        }
    }

    public class Scale2DProperty : Property<Vector2> {
        public override Vector2 GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale,
                Control control => control.RectScale,
                _ => throw new Exception($"Not Scale2D property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, Vector2 value) {
            if (node is Node2D node2D) node2D.Scale = value;
            else if (node is Control control) control.RectScale = value;
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D;
        }

        public override string ToString() {
            return "Scale2DProperty<Vector2>(node2D: \"Scale\", control: \"RectScale\")";
        }
    }

    public class ScaleXProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale.x,
                Control control => control.RectScale.x,
                Spatial spatial => spatial.Scale.x,
                _ => throw new Exception($"Not ScaleX property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D) node.SetIndexed("scale:x", value);
            else if (node is Control control) control.SetIndexed("rect_scale:x", value);
            else if (node is Spatial spatial) spatial.SetIndexed("scale:x", value);
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D || node is Spatial;
        }

        public override string ToString() {
            return "ScaleXProperty<float>(node2D: \"sale:x\", control: \"rect_scale:x\", spatial: \"scale:x\")";
        }
    }

    public class ScaleYProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Control control => control.RectScale.y,
                Node2D node2D => node2D.Scale.y,
                Spatial spatial => spatial.Scale.y,
                _ => throw new Exception($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D) node.SetIndexed("scale:y", value);
            else if (node is Control control) control.SetIndexed("rect_scale:y", value);
            else if (node is Spatial spatial) spatial.SetIndexed("scale:y", value);
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Spatial || node is Control || node is Node2D;
        }

        public override string ToString() {
            return "ScaleYProperty<float>(node2D: \"sale:y\", control: \"rect_scale:y\", spatial: \"scale:y\")";
        }
    }

    public class ScaleZProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale.z,
                _ => throw new Exception($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Spatial) node.SetIndexed("scale:z", value);
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }

        public override string ToString() {
            return "ScaleYProperty<float>(spatial: \"scale:z\")";
        }
    }

    public class IndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        public static implicit operator IndexedProperty<TProperty>(string from) => new IndexedProperty<TProperty>(from);

        public static implicit operator string(IndexedProperty<TProperty> from) => from._propertyName;

        private readonly NodePath _propertyName;

        public IndexedProperty(NodePath propertyName) {
            _propertyName = propertyName;
        }

        public virtual bool IsCompatibleWith(Node node) => true;

        public virtual TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedPropertyName(node));
        }

        public virtual void SetValue(AnimationContext<TProperty> context) {
            context.Target.SetIndexed(GetIndexedPropertyName(context.Target), context.Value);
        }

        public virtual void SetValue(Node target, TProperty value) {
            target.SetIndexed(GetIndexedPropertyName(target), value);
        }

        public virtual NodePath GetIndexedPropertyName(Node node) {
            return _propertyName;
        }

        public override string ToString() {
            return $"IndexedProperty<{typeof(TProperty).Name}>(\"{_propertyName}\")";
        }
    }

    public class ControlOrNode2DIndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        private readonly NodePath? _node2DProperty;
        private readonly NodePath? _controlProperty;

        public ControlOrNode2DIndexedProperty(NodePath node2DProperty, NodePath controlProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
        }

        public TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedPropertyName(node));
        }

        public void SetValue(AnimationContext<TProperty> context) {
            context.Target.SetIndexed(GetIndexedPropertyName(context.Target), context.Value);
        }

        public void SetValue(Node target, TProperty value) {
            target.SetIndexed(GetIndexedPropertyName(target), value);
        }

        public bool IsCompatibleWith(Node node) {
            return
                (node is Control && _controlProperty != null) ||
                (node is Node2D && _node2DProperty != null);
        }

        public NodePath GetIndexedPropertyName(Node node) {
            return node switch {
                Control _ when _controlProperty != null => _controlProperty,
                Node2D _ when _node2DProperty != null => _node2DProperty,
                _ => throw new ArgumentOutOfRangeException(nameof(node), node, "Only Node2D or Control nodes")
            };
        }

        public override string ToString() {
            return
                $"ControlOrNode2DIndexedProperty<{typeof(TProperty).Name}>(node2D: \"{_node2DProperty}\", control: \"{_controlProperty}\")";
        }
    }

    public abstract class ComputedProperty<TProperty> : IProperty<TProperty> {
        private readonly IProperty<TProperty> _realProperty;

        protected ComputedProperty(IProperty<TProperty> realProperty) {
            _realProperty = realProperty;
        }

        public TProperty GetValue(Node node) {
            return _realProperty.GetValue(node);
        }

        public void SetValue(AnimationContext<TProperty> context) {
            if (!IsCompatibleWith(context.Target)) return;
            var computeValue = ComputeValue(context.Target, context.InitialValue, context.Value);
            context.UpdateValue(_realProperty, computeValue);
        }

        protected abstract TProperty ComputeValue(Node node, TProperty initialValue, TProperty percent);

        public abstract bool IsCompatibleWith(Node node);
    }

    public class PositionBySizeX : ComputedProperty<float> {
        public PositionBySizeX(IProperty<float> realProperty) : base(realProperty) {
        }

        protected override float ComputeValue(Node node, float initialValue, float percent) {
            var size = node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            return initialValue + (size.x * percent);
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Sprite || node is Control;
        }

        public override string ToString() {
            return "PositionBySizeX(sprite, control)";
        }
    }

    public class PositionBySizeY : ComputedProperty<float> {
        public PositionBySizeY(IProperty<float> realProperty) : base(realProperty) {
        }

        protected override float ComputeValue(Node node, float initialValue, float percent) {
            var size = node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            return initialValue + (size.y * percent);
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Sprite || node is Control;
        }

        public override string ToString() {
            return "PositionBySizeY(sprite, control)";
        }
    }

    public class PositionBySize2D : ComputedProperty<Vector2> {
        public PositionBySize2D(IProperty<Vector2> realProperty) : base(realProperty) {
        }

        protected override Vector2 ComputeValue(Node node, Vector2 initialValue, Vector2 percent) {
            var size = node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            return initialValue + size * percent;
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Sprite || node is Control;
        }

        public override string ToString() {
            return "PositionBySize2D(sprite, control)";
        }
    }
}