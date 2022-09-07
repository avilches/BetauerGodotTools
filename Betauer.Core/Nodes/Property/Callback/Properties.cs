namespace Betauer.Nodes.Property.Callback {
    public static partial class CallbackProperties {

        /*
         * These are non-indexed properties (they use a callback Action to set the value) instead of the indexed version 
         */
        
        public static readonly Scale2DProperty Scale2D = new Scale2DProperty();
        public static readonly ScaleXProperty Scale2Dx = new ScaleXProperty();
        public static readonly ScaleYProperty Scale2Dy = new ScaleYProperty();
        public static readonly ScaleZProperty Scale3Dz = new ScaleZProperty();
        public static readonly Rotate2DProperty Rotate2D = new Rotate2DProperty();
        
    }
}