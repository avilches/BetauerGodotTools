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

        public static readonly IProperty<Vector2> Position2D =
            new ControlOrNode2DIndexedProperty<Vector2>("position", "rect_position");

        public static readonly IProperty<float> PositionX =
            new ControlOrNode2DIndexedProperty<float>("position:x", "rect_position:x");

        public static readonly IProperty<float> PositionXPercent = new PositionXPercent();

        public static readonly IProperty<float> PositionY =
            new ControlOrNode2DIndexedProperty<float>("position:y", "rect_position:y");

        public static readonly IProperty<float> PositionZ =
            new ControlOrNode2DIndexedProperty<float>("position:z", "rect_position:z");

        public static readonly IProperty<Vector2> Scale2D =
            new ControlOrNode2DIndexedProperty<Vector2>("scale", "rect_scale");

        public static readonly IProperty<float> ScaleX =
            new ControlOrNode2DIndexedProperty<float>("scale:x", "rect_scale:x");

        public static readonly IProperty<float> ScaleY =
            new ControlOrNode2DIndexedProperty<float>("scale:y", "rect_scale:y");

        public static readonly IProperty<float> ScaleZ =
            new ControlOrNode2DIndexedProperty<float>("scale:z", "rect_scale:z");

        public static readonly IProperty<float> RotateCenter =
            new ControlOrNode2DIndexedProperty<float>("rotation_degrees", "rect_rotation");

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

    public class PositionXPercent : IProperty<float> {
        public float GetValue(Node node) {
            return Property.PositionX.GetValue(node);
        }

        public void SetValue(Node node, float initialValue, float percent) {
            if (!IsCompatibleWith(node)) return;
            var size = node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            Property.PositionX.SetValue(node, initialValue, initialValue + (size.x * percent));
        }

        public bool IsCompatibleWith(Node node) {
            return node is Control || node is Sprite;
        }
    }
}