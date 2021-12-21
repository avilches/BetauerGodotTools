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

    /**
     * Special thanks to Alessandro Senese (Ceceppa)
     *
     * All the tricks to set pivots in Control nodes and create fake pivot in Sprite nodes are possible because
     * of his work in the wonderful library Anima: https://github.com/ceceppa/anima
     *
     * Thank you man! :)
     */
    public static class PropertyTools {
        public static Vector2 GetSpriteSize(this Sprite sprite) {
            return sprite.Texture.GetSize() * sprite.Scale;
        }

        public static void SetSpritePivot(this Sprite node2D, Vector2 offset) {
            var position = node2D.GlobalPosition;
            node2D.Offset = offset;
            node2D.GlobalPosition = position - node2D.Offset;
        }

        public static void SetPivotTopCenter(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x / 2, 0))
                control.RectPivotOffset = new Vector2(control.RectSize.x / 2, 0);
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(0, size.y / 2)
                SetSpritePivot(sprite, new Vector2(0, GetSpriteSize(sprite).y / 2));
            }
        }

        public static void SetPivotTopLeft(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(0, 0))
                control.RectPivotOffset = Vector2.Zero;
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(size.x / 2, 0)
                SetSpritePivot(sprite, new Vector2(GetSpriteSize(sprite).x / 2, 0));
            }
        }

        public static void SetPivotCenter(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(size / 2)
                control.RectPivotOffset = control.RectSize / 2;
            }
        }

        public static void SetPivotCenterBottom(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x / 2, size.y / 2))
                var size = control.RectSize;
                control.RectPivotOffset = new Vector2(size.x / 2, size.y / 2);
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(0, -size.y / 2)
                SetSpritePivot(sprite, new Vector2(0, -GetSpriteSize(sprite).y / 2));
            }
        }

        public static void SetPivotLeftBottom(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(0, size.y))
                control.RectPivotOffset = new Vector2(0, control.RectSize.y);
            } else if (node is Sprite sprite) {
                var size = GetSpriteSize(sprite);
                // node.offset = Vector2(size.x / 2, size.y)
                SetSpritePivot(sprite, new Vector2(size.x / 2, size.y));
            }
        }

        public static void SetPivotRightBottom(this Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x, size.y / 2))
                var size = control.RectSize;
                control.RectPivotOffset = new Vector2(size.x, size.y / 2);
            } else if (node is Sprite sprite) {
                var size = GetSpriteSize(sprite);
                // node.offset = Vector2(-size.x / 2, size.y / 2)
                SetSpritePivot(sprite, new Vector2(-size.x / 2, -size.y / 2));
            }
        }
    }
}