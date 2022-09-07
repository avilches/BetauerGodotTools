using System;
using Godot;

namespace Betauer.Nodes.Property {
    public static partial class Properties {
        public static readonly IndexedControlOrNode2DProperty<Color> Modulate =
            new IndexedControlOrNode2DProperty<Color>("modulate", "modulate");

        public static readonly IndexedControlOrNode2DProperty<float> ModulateR =
            new IndexedControlOrNode2DProperty<float>("modulate:r", "modulate:r");

        public static readonly IndexedControlOrNode2DProperty<float> ModulateG =
            new IndexedControlOrNode2DProperty<float>("modulate:g", "modulate:g");

        public static readonly IndexedControlOrNode2DProperty<float> ModulateB =
            new IndexedControlOrNode2DProperty<float>("modulate:b", "modulate:b");

        public static readonly IndexedControlOrNode2DProperty<float> Opacity =
            new IndexedControlOrNode2DProperty<float>("modulate:a", "modulate:a");

        /*
         * "transform.origin" matrix is used to change the position instead of "position:x" because it's safe to use
         * along with the SkewX/SkewY properties, which they use the transform.origin too.
         */
        public static readonly IndexedControlOrNode2DProperty<Vector2> Position2D =
            new IndexedControlOrNode2DProperty<Vector2>("transform:origin", "rect_position");

        public static readonly IndexedControlOrNode2DProperty<float> PositionX =
            new IndexedControlOrNode2DProperty<float>("transform:origin:x", "rect_position:x");

        public static readonly IndexedControlOrNode2DProperty<float> PositionY =
            new IndexedControlOrNode2DProperty<float>("transform:origin:y", "rect_position:y");

        public static readonly Func<Node, PositionBySizeX> PositionBySizeX = (node) => new PositionBySizeX(node);
        public static readonly Func<Node, PositionBySizeY> PositionBySizeY = (node) => new PositionBySizeY(node);
        public static readonly Func<Node, PositionBySize2D> PositionBySize2D = (node) => new PositionBySize2D(node);

        public static readonly IndexedControlOrNode2DProperty<Vector2> Scale2D =
            new IndexedControlOrNode2DProperty<Vector2>("scale", "rect_scale");

        public static readonly IndexedControlOrNode2DProperty<float> Scale2Dx =
            new IndexedControlOrNode2DProperty<float>("scale:x", "rect_scale:x");

        public static readonly IndexedControlOrNode2DProperty<float> Scale2Dy =
            new IndexedControlOrNode2DProperty<float>("scale:y", "rect_scale:y");

        /**
         * It doesn't work combined with Scale or Position (with transform)
         */
        public static readonly IndexedControlOrNode2DProperty<float> Rotate2D =
            new IndexedControlOrNode2DProperty<float>("rotation_degrees", "rect_rotation");

        public static readonly IndexedControlOrNode2DProperty<float> Skew2DX =
            new IndexedControlOrNode2DProperty<float>("transform:y:x", null);

        public static readonly IndexedControlOrNode2DProperty<float> Skew2DY =
            new IndexedControlOrNode2DProperty<float>("transform:x:y", null);
    }
}