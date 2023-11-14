using System;

namespace Betauer.Core.Data;

/// <summary>
/// Returns 1 in PositionX, PositionY (which is the center of the grid by default) and goes to 0 in the border in a linear way
/// </summary>
public class RectRampDataGrid : IDataGrid<float> {
    public int Width { get; set; }
    public int Height { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }


    public RectRampDataGrid(int width, int height) {
        Width = width;
        Height = height;
        PositionX = Width / 2;
        PositionY = Height / 2;
    }

    /// <summary>
    /// Center as a normalized value from 0 to 1, where 0.5 is the middle of the grid
    /// </summary>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    public void SetCenter(float centerX, float centerY) {
        PositionX = (int)(Math.Clamp(centerX, 0f, 1f) * Width);
        PositionY = (int)(Math.Clamp(centerY, 0f, 1f) * Height);
    }

    public void SetCenterPosition(int x, int y) {
        PositionX = Math.Clamp(x, 0, Width - 1);
        PositionY = Math.Clamp(y, 0, Height - 1);
    }

    public float GetValue(int x, int y) {
        var maxDistanceX = Math.Max(PositionX, Width - 1 - PositionX);
        var maxDistanceY = Math.Max(PositionY, Height - 1 - PositionY);

        var distanceX = (float)Math.Abs(x - PositionX);
        var distanceY = (float)Math.Abs(y - PositionY);

        var valueX = 1 - distanceX / maxDistanceX;
        var valueY = 1 - distanceY / maxDistanceY;
        return Math.Min(valueX, valueY);
    }
}