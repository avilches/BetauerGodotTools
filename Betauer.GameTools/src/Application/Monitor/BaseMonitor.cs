using Betauer.Core.Nodes;

namespace Betauer.Application.Monitor; 

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract partial class BaseMonitor : VBoxContainer {

    public static readonly Color DefaultSeparatorColor = new(1,1,1,0.05f);
    public static readonly Color DefaultBorderColor = new(1,1,1,0.1f);
    public static readonly Color DefaultErrorColor = Colors.Red;
        
    public abstract void CheckProcessBasedOnVisibility();

    public static List<Color> Palette;
    private static int _nextColor = 0; 
    static BaseMonitor() {
        // More palettes for charts in: https://www.heavy.ai/blog/12-color-palettes-for-telling-better-stories-with-your-data
        var palette = new List<string> {
            // Dutch Field
            "#e60049", "#0bb4ff", "#50e991", "#e6d800", "#9b19f5", "#ffa300", "#dc0ab4", "#b3d4ff", "#00bfa0"
            // Retro Metro
            // "#ea5545", "#f46a9b", "#ef9b20", "#edbf33", "#ede15b", "#bdcf32", "#87bc45", "#27aeef", "#b33dc6"
        };
        Palette = palette.Select(c => new Color(c)).ToList();
    }

    public Color NextColor() {
        return Palette[_nextColor++ % Palette.Count];
    }
}

public abstract partial class BaseMonitor<TBuilder> : BaseMonitor where TBuilder : class {
    private double _timeElapsed = 0;
    private double _updateEvery = 0;
    private bool _updateOnPhysics = true;
    public Func<bool>? DestroyIfFunc { get; private set; }
    public GodotObject? Target { get; private set; }
    public GodotObject? ParentTarget { get; private set; }

    protected BaseMonitor() {
        Ready += () => {
            CheckProcessBasedOnVisibility();
            ParentTarget = this.FindParent<DebugOverlay>()?.Target;
        };
        VisibilityChanged += CheckProcessBasedOnVisibility;
        ProcessMode = ProcessModeEnum.Pausable;
    }

    public TBuilder DestroyIf(Func<bool> destroyIf) {
        DestroyIfFunc = destroyIf;
        return this as TBuilder;
    }

    public TBuilder Follow(GodotObject godotObject) {
        Target = godotObject;
        return this as TBuilder;
    }

    public TBuilder Enable(bool enable = true) {
        Visible = enable;
        return this as TBuilder;
    }

    public TBuilder Disable() {
        return Enable(false);
    }

    public override void CheckProcessBasedOnVisibility() {
        var isVisible = IsVisibleInTree();
        if (isVisible) _timeElapsed = 0; // this ensure next time when the monitor is visible, the data is updated immediately
        SetPhysicsProcess(isVisible && _updateOnPhysics);
        SetProcess(isVisible && !_updateOnPhysics);
    }

    public TBuilder UpdateEvery(float time) {
        _updateEvery = Math.Max(time, 0);
        return this as TBuilder;
    }

    public TBuilder UpdateOnPhysics(bool updateOnPhysics = true) {
        _updateOnPhysics = updateOnPhysics;
        CheckProcessBasedOnVisibility();
        return this as TBuilder;
    }

    public override void _Process(double delta) {
        if (_updateOnPhysics) CheckProcessBasedOnVisibility();
        Process(delta);
    }

    public override void _PhysicsProcess(double delta) {
        if (!_updateOnPhysics) CheckProcessBasedOnVisibility();
        Process(delta);
    }

    private void Process(double delta) {
        if ((Target != null && !IsInstanceValid(Target)) ||
            (ParentTarget != null && !IsInstanceValid(ParentTarget)) ||
            (DestroyIfFunc != null && DestroyIfFunc())) {
            QueueFree();
        } else if (_updateEvery > 0) {
            if (_timeElapsed == 0) {
                _timeElapsed += delta;
                UpdateMonitor(delta);
            } else {
                _timeElapsed += delta;
                if (_timeElapsed >= _updateEvery) {
                    UpdateMonitor(delta);
                    _timeElapsed -= _updateEvery;
                }
            }
        } else {
            UpdateMonitor(delta);
        }
    }

    public abstract void UpdateMonitor(double delta);
}