using System;
using Godot;

namespace Betauer.Core.Nodes.Property {
    public static partial class Properties {
        public static readonly IndexedProperty<Color> Modulate = IndexedSingleProperty.Create<Color>("modulate", typeof(CanvasItem));
        public static readonly IndexedProperty<float> ModulateR = IndexedSingleProperty.Create<float>("modulate:r", typeof(CanvasItem));
        public static readonly IndexedProperty<float> ModulateG = IndexedSingleProperty.Create<float>("modulate:g", typeof(CanvasItem));
        public static readonly IndexedProperty<float> ModulateB = IndexedSingleProperty.Create<float>("modulate:b", typeof(CanvasItem));
        public static readonly IndexedProperty<float> Opacity = IndexedSingleProperty.Create<float>("modulate:a", typeof(CanvasItem));

        /*
         * "transform.origin" matrix is used to change the position instead of "position:x" because it's safe to use
         * along with the SkewX/SkewY properties, which they use the transform.origin too.
         */
        // TODO Godot 4: check if this still works or it can be merged into a Node2D "position"
        public static readonly IndexedControlOrNode2DProperty<Vector2> Position2D = new("transform:origin", "position");
        public static readonly IndexedControlOrNode2DProperty<float> PositionX = new("transform:origin:x", "position:x");
        public static readonly IndexedControlOrNode2DProperty<float> PositionY = new("transform:origin:y", "position:y");

        public static readonly Func<Node, PositionBySizeX> PositionBySizeX = (node) => new(node);
        public static readonly Func<Node, PositionBySizeY> PositionBySizeY = (node) => new(node);
        public static readonly Func<Node, PositionBySize2D> PositionBySize2D = (node) => new(node);

        public static readonly IndexedControlOrNode2DProperty<Vector2> Scale2D = new("scale", "scale");
        public static readonly IndexedControlOrNode2DProperty<float> Scale2Dx = new("scale:x", "scale:x");
        public static readonly IndexedControlOrNode2DProperty<float> Scale2Dy = new("scale:y", "scale:y");

        /**
         * It doesn't work combined with Scale or Position (with transform)
         */
        public static readonly IndexedControlOrNode2DProperty<float> Rotate2D = new("rotation", "rotation");
        public static readonly IndexedControlOrNode2DProperty<float> Skew2DX = new("transform:y:x", null);
        public static readonly IndexedControlOrNode2DProperty<float> Skew2DY = new("transform:x:y", null);
    }
}