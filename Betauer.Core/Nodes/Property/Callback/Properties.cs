namespace Betauer.Core.Nodes.Property.Callback; 

public static partial class CallbackProperties {

    /*
     * These are non-indexed properties (they use a callback Action to set the value) instead of the indexed version 
     */
        
    public static readonly Scale2DProperty Scale2D = new();
    public static readonly ScaleXProperty Scale2Dx = new();
    public static readonly ScaleYProperty Scale2Dy = new();
    public static readonly ScaleZProperty Scale3Dz = new();
    public static readonly Rotate2DProperty Rotate2D = new();
        
}