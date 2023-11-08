using Godot;

namespace Betauer.Core.Data;

public class FalloffDataGrid : IDataGrid<float> {
    public int Width { get; set; }
    public int Height { get; set; }
    public float Exp { get; set; }
    public float Offset { get; set; }

    public FalloffDataGrid(int width, int height) : this(width, height, 3, 5) {
    }
    
    public FalloffDataGrid(int width, int height, float exp, float offset) {
        Width = width;
        Height = height;
        Exp = exp;
        Offset = offset;
    }
    
    public float GetValue(int x, int y) {
        var xx = x / (float)Width * 2 - 1;
        var yy = y / (float)Height * 2 - 1;
        var value = Mathf.Max(Mathf.Abs(xx), Mathf.Abs(yy));
        var pow = Mathf.Pow(value, Exp);
        return pow / (pow + Mathf.Pow(Offset - Offset * value, Exp));
    }
}