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

        public static readonly IProperty<float>
            Opacity = new ControlOrNode2DIndexedProperty<float>("modulate:a", "modulate:a");

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

        public static readonly IProperty<float> PositionZ =
            new ControlOrNode2DIndexedProperty<float>("transform:origin:z", "rect_position:z");

        // These PercentPositionX and PercentPositionY constructors need to be located after the PositionX and PositionY
        public static readonly IProperty<float> PercentPositionX = new PercentPositionX();

        public static readonly IProperty<float> PercentPositionY = new PercentPositionY();

        public static readonly IProperty<Vector2> PercentPosition2D = new PercentPosition2D();

        public static readonly IProperty<Vector2> Scale2D =
            new ControlOrNode2DIndexedProperty<Vector2>("scale", "rect_scale");

        public static readonly IProperty<float> ScaleX =
            new ControlOrNode2DIndexedProperty<float>("scale:x", "rect_scale:x");

        public static readonly IProperty<float> ScaleY =
            new ControlOrNode2DIndexedProperty<float>("scale:y", "rect_scale:y");

        public static readonly IProperty<float> ScaleZ =
            new ControlOrNode2DIndexedProperty<float>("scale:z", "rect_scale:z");

        /**
         * Why use a class to update the RotationDegrees property instead of this?

            public static readonly IProperty<float> RotateCenter =
                    new ControlOrNode2DIndexedProperty<float>("rotation_degrees", "rect_rotation");

         * The problem is:
         *
         * LightSpeed animation needs move position and skew:
         * - position:x + skew:x/skew.y -> IT DOESN'T WORK (it scale the sprite too, which is not expected)
         * - transform:origin:x + skew:x/skew:y  -> WORKS OK
         *
         * RollIn animation needs move position and rotation:
         * - "position:x" + "rotation_degrees" -> WORKS OK
         * - "transform:origin:x" + "rotation_degrees" -> IT DOESN'T WORK (it scale the sprite too, which is not expected)
         *
         * JackInTheBox animation needs scale and rotate:
         * - "scale" + "rotation_degrees" -> IT DOESN'T WORK (it scale with bigger values)
         *
         * So, in order to allow the 3 animations work using just one type of every property is:
         * 1. Use "transform:origin:x" which works well with skew
         * 2. Change the rotation_degrees with a this function, which allow to work with scale (JackInTheBox and move (RollIn)
         * 
         */
        public static readonly IProperty<float> RotateCenter = new RotateProperty();


        public static readonly IProperty<float> SkewX =
            new ControlOrNode2DIndexedProperty<float>("transform:y:x", null);

        public static readonly IProperty<float> SkewY =
            new ControlOrNode2DIndexedProperty<float>("transform:x:y", null);
    }

    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public TProperty GetValue(Node node);
        public void SetValue(Node node, TProperty initialValue, TProperty value);
        public bool IsCompatibleWith(Node node);
    }

    public class RotateProperty : IProperty<float> {
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

        public bool IsCompatibleWith(Node node) => true;

        public TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedProperty(node));
        }

        public void SetValue(Node node, TProperty initialValue, TProperty value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }

        public string GetIndexedProperty(Node node) {
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

    public class PercentPositionX : ComputedProperty<float> {
        public PercentPositionX() : base(Property.PositionX) {
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

    public class PercentPositionY : ComputedProperty<float> {
        public PercentPositionY() : base(Property.PositionY) {
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

    public class PercentPosition2D : ComputedProperty<Vector2> {
        public PercentPosition2D() : base(Property.Position2D) {
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