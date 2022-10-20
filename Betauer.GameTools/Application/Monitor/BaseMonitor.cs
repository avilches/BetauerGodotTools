using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public abstract class BaseMonitor : VBoxContainer {

        public static readonly Color DefaultSeparatorColor = new(1,1,1,0.05f);
        public static readonly Color DefaultBorderColor = new(1,1,1,0.1f);
        public static readonly Color DefaultLabelColor = new Color(0.584314f, 0.584314f, 0.584314f, 1);
        
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

    public abstract class BaseMonitor<TBuilder> : BaseMonitor where TBuilder : class {
        public Object? Watch { get; set; }
        public Func<bool>? RemoveIfFunc { get; private set; }

        public TBuilder Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
            return this as TBuilder;
        }

        public TBuilder RemoveIfInvalid(Object target) {
            Watch = target;
            return this as TBuilder;
        }

        public TBuilder RemoveIf(Func<bool> func) {
            RemoveIfFunc = func;
            return this as TBuilder;
        }

        public TBuilder Disable() {
            return Enable(false);
        }
        
        public override void _PhysicsProcess(float delta) {
            var watching = Watch ?? DebugOverlayOwner.Target;
            if ((watching != null && !IsInstanceValid(watching)) || (RemoveIfFunc != null && RemoveIfFunc())) {
                QueueFree();
            } else if (!Visible) {
                Disable();
            } else {
                Process(delta);
            }
        }

        public abstract void Process(float delta);
    }
}