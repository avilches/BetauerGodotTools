namespace Betauer.Core.PCG.Examples.Metazelda.util;

public class MZRect
{
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }

    public MZRect(int left, int top, int right, int bottom)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }
}