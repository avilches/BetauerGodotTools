namespace Betauer.Application.Monitor; 

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract partial class BaseMonitor : VBoxContainer {

    public static readonly Color DefaultSeparatorColor = new(1,1,1,0.05f);
    public static readonly Color DefaultBorderColor = new(1,1,1,0.1f);
    public static readonly Color DefaultLabelColor = new(0.584314f, 0.584314f, 0.584314f, 1);
        
    public bool IsEnabled => Visible;
    public DebugOverlay DebugOverlayOwner { get; set; }


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

    public DebugOverlay EndMonitor() {
        return DebugOverlayOwner;
    }
}

public abstract partial class BaseMonitor<TBuilder> : BaseMonitor where TBuilder : class {
    private double _timeElapsed = 0;
    private double _updateEvery = 0;
    public Func<bool>? RemoveIfFunc { get; private set; }

    public TBuilder Enable(bool enable = true) {
        Visible = enable;
        SetPhysicsProcess(enable);
        return this as TBuilder;
    }

    public TBuilder RemoveIf(Func<bool> func) {
        RemoveIfFunc = func;
        return this as TBuilder;
    }

    public TBuilder Disable() {
        return Enable(false);
    }
        
    public TBuilder UpdateEvery(float time) {
        _updateEvery = Math.Max(time, 0);
        return this as TBuilder;
    }

    public override void _PhysicsProcess(double delta) {
        var watching = DebugOverlayOwner.Target;
        if ((watching != null && !IsInstanceValid(watching)) || (RemoveIfFunc != null && RemoveIfFunc())) {
            QueueFree();
        } else if (!Visible) {
            Disable();
        } else {
            if (DebugOverlayOwner.Visible) {
                if (_updateEvery > 0) {
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
            } else {
                _timeElapsed = 0;
            }
        }
    }

    public abstract void UpdateMonitor(double delta);
}