using System;
using Godot;

namespace Betauer.Application.SplitScreen;

public abstract class BaseSplitScreen {
    public Func<Vector2> GetParentSize { get; set; }
    private bool _configured = false;
    private bool _split = false;
    private bool _horizontal = false;

    public event Action<bool> OnChange;
    
    public bool Horizontal {
        get => _horizontal;
        set {
            if (_configured && _horizontal == value) return;
            _horizontal = value;
            Refresh();
        }
    }

    public bool Split {
        get => _split;
        set {
            if (_configured && _split == value) return;
            _split = value;
            Refresh();
        }
    }

    public void Refresh() {
        if (GetParentSize == null) throw new Exception("Please, define a valid GetParentSize function!");
        _configured = _split ? ToSplitScreen() : ToSingle();
        if (_configured) {
            OnChange?.Invoke(_split);
        }
    }

    protected abstract bool ToSplitScreen();
    protected abstract bool ToSingle();
}