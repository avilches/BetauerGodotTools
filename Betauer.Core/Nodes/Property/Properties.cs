using System;
using Godot;

namespace Betauer.Nodes.Property {
    public static partial class Properties {
        public static readonly ControlOrNode2DIndexedProperty<Color> Modulate =
            new ControlOrNode2DIndexedProperty<Color>("modulate", "modulate");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateR =
            new ControlOrNode2DIndexedProperty<float>("modulate:r", "modulate:r");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateG =
            new ControlOrNode2DIndexedProperty<float>("modulate:g", "modulate:g");

        public static readonly ControlOrNode2DIndexedProperty<float> ModulateB =
            new ControlOrNode2DIndexedProperty<float>("modulate:b", "modulate:b");

        public static readonly ControlOrNode2DIndexedProperty<float> Opacity =
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

        // To avoid NPE, put these PercentPositionX, PercentPositionY and PositionBySize2D objects
        // just before the PositionX, PositionY and PositionBySize2D
        public static readonly Func<Node, PositionBySizeX> PositionBySizeX = (node) => new PositionBySizeX(node);
        public static readonly Func<Node, PositionBySizeY> PositionBySizeY = (node) => new PositionBySizeY(node);
        public static readonly Func<Node, PositionBySize2D> PositionBySize2D = (node) => new PositionBySize2D(node);

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
}