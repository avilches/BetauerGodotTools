using System;
using Godot;

namespace Betauer.Application.SplitScreen;

public class SplitScreenContainer<T> where T : Control {
    private bool _split;
    private bool _horizontal = true;

    public T Split1 { get; set; }
    public T Split2 { get; set; }
    public Func<Vector2> GetSize { get; set; }

    public bool Horizontal {
        get => _horizontal;
        set {
            if (_horizontal == value) return;
            _horizontal = value;
            Refresh();
        }
    }

    public bool Split {
        get => _split;
        set {
            if (_split == value) return;
            _split = value;
            Refresh();
        }
    }

    public void Refresh() {
        if (_split) ToSplitScreen();
        else ToSingle();
    }

    private void ToSingle() {
        if (Split1 == null) return;
        if (GetSize == null) throw new Exception("Please, define a valid GetSize function!");

        var windowSize = GetSize();
        Split1.Position = Vector2.Zero;
        Split1.CustomMinimumSize = windowSize;
        Split1.Visible = true;
        Split2.Visible = false;
    }

    private void ToSplitScreen() {
        if (Split1 == null || Split2 == null) return;
        if (GetSize == null) throw new Exception("Please, define a valid GetSize function!");

        var windowSize = GetSize();
        var splitSize = Horizontal ? new Vector2(windowSize.X, windowSize.Y / 2) : new Vector2(windowSize.X / 2, windowSize.Y);

        Split1.Position = Vector2.Zero;
        Split2.Position = Horizontal ? new Vector2(0, windowSize.Y / 2) : new Vector2(windowSize.X / 2, 0);
        Split1.CustomMinimumSize = splitSize;
        Split2.CustomMinimumSize = splitSize;
        Split1.Visible = true;
        Split2.Visible = true;
    }
}