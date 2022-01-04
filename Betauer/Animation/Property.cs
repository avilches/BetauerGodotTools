using System;
using Godot;

namespace Betauer.Animation {
    public static class Property {
        public static readonly IProperty<Color> Modulate =
            new ControlOrNode2DIndexedProperty<Color>("modulate", "modulate");

        public static readonly IProperty<float> ModulateR =
            new ControlOrNode2DIndexedProperty<float>("modulate:r", "modulate:r");

        public static readonly IProperty<float> ModulateG =
            new ControlOrNode2DIndexedProperty<float>("modulate:g", "modulate:g");

        public static readonly IProperty<float> ModulateB =
            new ControlOrNode2DIndexedProperty<float>("modulate:b", "modulate:b");

        public static readonly IProperty<float> Opacity =
            new ControlOrNode2DIndexedProperty<float>("modulate:a", "modulate:a");

        /*
         * "transform.origin" matrix is used to change the position instead of "position:x" because it's safe to use
         * along with the SkewX/SkewY properties, which they use the transform.origin too.
         */
        public static readonly IProperty<Vector2> Position2D =
            new ControlOrNode2DIndexedProperty<Vector2>("transform:origin", "rect_position");

        public static readonly IProperty<float> PositionX =
            new ControlOrNode2DIndexedProperty<float>("transform:origin:x", "rect_position:x");

        public static readonly IProperty<float> PositionY =
            new ControlOrNode2DIndexedProperty<float>("transform:origin:y", "rect_position:y");

        // These PercentPositionX and PercentPositionY constructors need to be located after the PositionX and PositionY
        public static readonly IProperty<float> PositionBySizeX = new PositionBySizeX();

        public static readonly IProperty<float> PositionBySizeY = new PositionBySizeY();

        public static readonly IProperty<Vector2> PositionBySize2D = new PositionBySize2D();

        public static readonly IProperty<Vector2> Scale2D =
            new ControlOrNode2DIndexedProperty<Vector2>("scale", "rect_scale");

        public static readonly IProperty<float> Scale2DX =
            new ControlOrNode2DIndexedProperty<float>("scale:x", "rect_scale:x");

        public static readonly IProperty<float> Scale2DY =
            new ControlOrNode2DIndexedProperty<float>("scale:y", "rect_scale:y");

        public static readonly IProperty<Vector2> Scale2DByCallback = new Scale2DProperty();
        public static readonly IProperty<float> Scale2DXByCallback = new ScaleXProperty();
        public static readonly IProperty<float> Scale2DYByCallback = new ScaleYProperty();

        public static readonly IProperty<float> Scale3DZByCallback = new ScaleZProperty();

        /**
         * It doesn't work combined with Scale or Position (with transform)
         */
        public static readonly IProperty<float> Rotate2D =
            new ControlOrNode2DIndexedProperty<float>("rotation_degrees", "rect_rotation");

        public static readonly IProperty<float> Rotate2DByCallback = new Rotate2DProperty();

        public static readonly IProperty<float> Skew2DX =
            new ControlOrNode2DIndexedProperty<float>("transform:y:x", null);

        public static readonly IProperty<float> Skew2DY =
            new ControlOrNode2DIndexedProperty<float>("transform:x:y", null);
    }

    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public TProperty GetValue(Node node);
        public void SetValue(Node node, TProperty initialValue, TProperty value);
        public bool IsCompatibleWith(Node node);
    }

    public class CallbackProperty<TProperty> : IProperty<TProperty> {
        private readonly Action<TProperty> _action;

        public CallbackProperty(Action<TProperty> action) {
            _action = action;
        }

        public TProperty GetValue(Node node) {
            return default;
        }

        public void SetValue(Node node, TProperty initialValue, TProperty value) {
            _action.Invoke(value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
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

        public void SetValue(Node node, TProperty initialValue, TProperty value) {
            _action.Invoke(node, value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }
    }


    public class Rotate2DProperty : IProperty<float> {
        public float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.RotationDegrees,
                Control control => control.RectRotation,
                _ => throw new Exception($"Not rotation property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, float initialValue, float value) {
            if (node is Node2D node2D) node2D.RotationDegrees = value;
            else if (node is Control control) control.RectRotation = value;
        }

        public bool IsCompatibleWith(Node node) {
            if (node is Control) return true;
            if (node is Node2D) return true;
            return false;
        }
    }

    public class Scale3DProperty : IProperty<Vector3> {
        public Vector3 GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale,
                _ => throw new Exception($"Not Scale3D property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, Vector3 initialValue, Vector3 value) {
            if (node is Spatial spatial) spatial.Scale = value;
        }

        public bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }
    }

    public class Scale2DProperty : IProperty<Vector2> {
        public Vector2 GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale,
                Control control => control.RectScale,
                _ => throw new Exception($"Not Scale2D property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, Vector2 initialValue, Vector2 value) {
            if (node is Node2D node2D) node2D.Scale = value;
            else if (node is Control control) control.RectScale = value;
        }

        public bool IsCompatibleWith(Node node) {
            if (node is Control) return true;
            if (node is Node2D) return true;
            return false;
        }
    }

    public class ScaleXProperty : IProperty<float> {
        public float GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale.x,
                Node2D node2D => node2D.Scale.x,
                Control control => control.RectScale.x,
                _ => throw new Exception($"Not ScaleX property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, float initialValue, float value) {
            if (node is Node2D || node is Spatial) node.SetIndexed("scale:x", value);
            else if (node is Control control) control.SetIndexed("rect_scale:x", value);
        }

        public bool IsCompatibleWith(Node node) {
            if (node is Control) return true;
            if (node is Node2D) return true;
            return false;
        }
    }

    public class ScaleYProperty : IProperty<float> {
        public float GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale.y,
                Node2D node2D => node2D.Scale.y,
                Control control => control.RectScale.y,
                _ => throw new Exception($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, float initialValue, float value) {
            if (node is Node2D || node is Spatial) node.SetIndexed("scale:y", value);
            else if (node is Control control) control.SetIndexed("rect_scale:y", value);
        }

        public bool IsCompatibleWith(Node node) {
            return node is Spatial || node is Control || node is Node2D;
        }
    }

    public class ScaleZProperty : IProperty<float> {
        public float GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale.z,
                _ => throw new Exception($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public void SetValue(Node node, float initialValue, float value) {
            if (node is Spatial) node.SetIndexed("scale:z", value);
        }

        public bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }
    }

    public interface IIndexedProperty<TProperty> : IProperty<TProperty> {
        public string GetIndexedProperty(Node node);
    }

    public class IndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        public static implicit operator IndexedProperty<TProperty>(string from) => new IndexedProperty<TProperty>(from);

        public static implicit operator string(IndexedProperty<TProperty> from) => from._propertyName;

        private readonly string _propertyName;

        public IndexedProperty(string propertyName) {
            _propertyName = propertyName;
        }

        public virtual bool IsCompatibleWith(Node node) => true;

        public virtual TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedProperty(node));
        }

        public virtual void SetValue(Node node, TProperty initialValue, TProperty value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }

        public virtual string GetIndexedProperty(Node node) {
            return _propertyName;
        }

        public override string ToString() {
            return _propertyName;
        }
    }

    public class ControlOrNode2DIndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        private readonly string _node2DProperty;
        private readonly string _controlProperty;

        public ControlOrNode2DIndexedProperty(string node2DProperty, string controlProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
        }

        public TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedProperty(node));
        }

        public virtual void SetValue(Node node, TProperty initialValue, TProperty value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }

        public virtual bool IsCompatibleWith(Node node) {
            if (node is Control && _controlProperty != null) return true;
            if (node is Node2D && _node2DProperty != null) return true;
            return false;
        }

        public string GetIndexedProperty(Node node) {
            return node switch {
                Control control => _controlProperty,
                Node2D node2D => _node2DProperty,
            };
        }

        public override string ToString() {
            return $"Control:{_controlProperty}, Node2D:{_node2DProperty}";
        }
    }

    public abstract class ComputedProperty<TProperty> : IProperty<TProperty> {
        private readonly IProperty<TProperty> _property;

        public ComputedProperty(IProperty<TProperty> property) {
            _property = property;
        }

        public TProperty GetValue(Node node) {
            return _property.GetValue(node);
        }

        public void SetValue(Node node, TProperty initialValue, TProperty percent) {
            if (!IsCompatibleWith(node)) return;
            _property.SetValue(node, initialValue, ComputeValue(node, initialValue, percent));
        }

        protected abstract TProperty ComputeValue(Node node, TProperty initialValue, TProperty percent);

        public abstract bool IsCompatibleWith(Node node);
    }

    public class PositionBySizeX : ComputedProperty<float> {
        public PositionBySizeX() : base(Property.PositionX) {
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
    }

    public class PositionBySizeY : ComputedProperty<float> {
        public PositionBySizeY() : base(Property.PositionY) {
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
    }

    public class PositionBySize2D : ComputedProperty<Vector2> {
        public PositionBySize2D() : base(Property.Position2D) {
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
    }
}