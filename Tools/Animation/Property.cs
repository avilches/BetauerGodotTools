using Godot;

namespace Tools.Animation {
    public static class Property {
        public static readonly IProperty<Color> Modulate = new Property<Color>("modulate");
        public static readonly IProperty<float> ModulateR = new Property<float>("modulate:r");
        public static readonly IProperty<float> ModulateG = new Property<float>("modulate:g");
        public static readonly IProperty<float> ModulateB = new Property<float>("modulate:b");
        public static readonly IProperty<float> Opacity = new Property<float>("modulate:a");

        public static readonly IProperty<Vector2> Position2D = new Position2DProperty();
        public static readonly IProperty<float> PositionX = new PositionProperty("x");
        public static readonly IProperty<float> PositionY = new PositionProperty("y");
        public static readonly IProperty<float> PositionZ = new PositionProperty("z"); // TODO: not tested

        public static readonly IProperty<Vector2> Scale2D = new Scale2DProperty();
        public static readonly IProperty<float> ScaleX = new ScaleProperty("x");
        public static readonly IProperty<float> ScaleY = new ScaleProperty("y");
        public static readonly IProperty<float> ScaleZ = new ScaleProperty("z"); // TODO: not tested

        public static readonly IProperty<float> RotateCenter = new RotationProperty();
    }

    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public abstract TProperty GetValue(Node node);
        public abstract void SetValue(Node node, TProperty value);
    }

    public abstract class IndexedProperty<TProperty> : IProperty<TProperty> {
        public abstract string GetIndexedProperty(Node node);

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
    }

    public class Scale2DProperty : IndexedProperty<Vector2> {
        public override string GetIndexedProperty(Node node) {
            return node is Control ? "rect_scale" : "scale";
        }
    }

    public class ScaleProperty : IndexedProperty<float> {
        public readonly string Key;

        public ScaleProperty(string key) {
            Key = key;
        }

        public override string GetIndexedProperty(Node node) {
            return node is Control ? "rect_scale:" + Key : "scale:" + Key;
        }
    }

    public class Position2DProperty : IndexedProperty<Vector2> {
        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_position",
                Node2D node2D => "position",
                _ => "global_transform:origin" // TODO: case not tested
            };
        }
    }

    public class RotationProperty : IndexedProperty<float> {
        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_rotation",
                Node2D node2D => "rotation_degrees",
                _ => "rotation" // TODO: case not tested
            };
        }
    }

    public class PositionProperty : IndexedProperty<float> {
        private readonly string _key;

        public PositionProperty(string key) {
            _key = key;
        }

        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_position:" + _key,
                Node2D node2D => "position:" + _key,
                _ => "global_transform:origin:" + _key // TODO: this case is not tested... 3D?
            };
        }
    }

}