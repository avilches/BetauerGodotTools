using Godot;

namespace Betauer.Animation {
    public static class Property {
        public static readonly IProperty<Color> Modulate = new ControlOrNode2DProperty<Color>("modulate", "modulate");

        public static readonly IProperty<float> ModulateR =
            new ControlOrNode2DProperty<float>("modulate:r", "modulate:r");

        public static readonly IProperty<float> ModulateG =
            new ControlOrNode2DProperty<float>("modulate:g", "modulate:g");

        public static readonly IProperty<float> ModulateB =
            new ControlOrNode2DProperty<float>("modulate:b", "modulate:b");

        public static readonly IProperty<float>
            Opacity = new ControlOrNode2DProperty<float>("modulate:a", "modulate:a");

        public static readonly IProperty<Vector2> Position2D =
            new ControlOrNode2DProperty<Vector2>("position",
                "rect_position"); // TODO: enable & test ,"global_transform:origin");

        public static readonly IProperty<float> PositionX =
            new ControlOrNode2DProperty<float>("position:x",
                "rect_position:x"); // TODO: enable & test ,"global_transform:origin:x");

        public static readonly IProperty<float> PositionY =
            new ControlOrNode2DProperty<float>("position:y",
                "rect_position:y"); // TODO: enable & test , "global_transform:origin:y");

        public static readonly IProperty<float> PositionZ =
            new ControlOrNode2DProperty<float>("position:z",
                "rect_position:z"); // TODO: enable & test , "global_transform:origin:z");

        public static readonly IProperty<Vector2> Scale2D =
            new ControlOrNode2DProperty<Vector2>("scale", "rect_scale");

        public static readonly IProperty<float> ScaleX =
            new ControlOrNode2DProperty<float>("scale:x", "rect_scale:x");

        public static readonly IProperty<float> ScaleY =
            new ControlOrNode2DProperty<float>("scale:y", "rect_scale:y");

        public static readonly IProperty<float> ScaleZ =
            new ControlOrNode2DProperty<float>("scale:z", "rect_scale:z");

        public static readonly IProperty<float> RotateCenter =
            new ControlOrNode2DProperty<float>("rotation_degrees",
                "rect_rotation"); // TODO: enable & test , "rotation");

        public static readonly IProperty<float> SkewX =
            new ControlOrNode2DProperty<float>("transform:y:x", null);

        public static readonly IProperty<float> SkewY =
            new ControlOrNode2DProperty<float>("transform:x:y", null);
    }

    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public TProperty GetValue(Node node);
        public void SetValue(Node node, TProperty value);
        public bool IsCompatibleWith(Node node);
    }

    public abstract class IndexedProperty<TProperty> : IProperty<TProperty> {
        public abstract string GetIndexedProperty(Node node);
        public virtual bool IsCompatibleWith(Node node) => true;

        public TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedProperty(node));
        }

        public void SetValue(Node node, TProperty value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }
    }

    public class Property<TProperty> : IndexedProperty<TProperty> {
        public static implicit operator Property<TProperty>(string from) => new Property<TProperty>(from);

        public static implicit operator string(Property<TProperty> from) => from.IndexedProperty;

        public readonly string IndexedProperty;

        public Property(string indexedProperty) {
            IndexedProperty = indexedProperty;
        }

        public override string GetIndexedProperty(Node node) {
            return IndexedProperty;
        }

        public override string ToString() {
            return IndexedProperty;
        }
    }

    public class ControlOrNode2DProperty<T> : IndexedProperty<T> {
        private readonly string _node2DProperty;
        private readonly string _controlProperty;
        private readonly string _nodeProperty;

        public ControlOrNode2DProperty(string node2DProperty, string controlProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
        }

        public ControlOrNode2DProperty(string node2DProperty, string controlProperty, string nodeProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
            _nodeProperty = nodeProperty;
        }

        public override bool IsCompatibleWith(Node node) {
            if (node is Control && _controlProperty != null) return true;
            if (node is Node2D && _node2DProperty != null) return true;
            if (_nodeProperty != null) return true;
            return false;
        }


        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => _controlProperty,
                Node2D node2D => _node2DProperty,
                _ => _nodeProperty
            };
        }

        public override string ToString() {
            return $"Control:{_controlProperty}, Node2D:{_node2DProperty}, Node: {_nodeProperty}";
        }
    }
}