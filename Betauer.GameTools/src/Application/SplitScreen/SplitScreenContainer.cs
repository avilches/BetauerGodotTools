using System;
using Godot;

namespace Betauer.Application.SplitScreen;

public class SplitScreenContainer<T> : BaseSplitScreen where T : Control {

    public T Split1 { get; set; }
    public T Split2 { get; set; }

    public SplitScreenContainer(Func<Vector2> getParentSize) {
        GetParentSize = getParentSize;
    }

    protected override bool ToSingle() {
        if (Split1 == null) return false;

        var windowSize = GetParentSize();
        Split1.Position = Vector2.Zero;
        Split1.CustomMinimumSize = windowSize;
        Split1.Visible = true;
        Split2.Visible = false;
        return true;
    }

    protected override bool ToSplitScreen() {
        if (Split1 == null || Split2 == null) return false;

        var windowSize = GetParentSize();
        var splitSize = Horizontal ? new Vector2(windowSize.X, windowSize.Y / 2) : new Vector2(windowSize.X / 2, windowSize.Y);

        Split1.Position = Vector2.Zero;
        Split2.Position = Horizontal ? new Vector2(0, windowSize.Y / 2) : new Vector2(windowSize.X / 2, 0);
        Split1.CustomMinimumSize = Split2.CustomMinimumSize = splitSize;
        Split1.Visible = Split2.Visible = true;
        return true;
    }
}