using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Managers;

namespace Veronenger.Controller.UI.Consoles {
    public abstract class ConsoleButton : Sprite {
        private SpriteConfig _config;

        [Inject] protected Xbox360SpriteConfig Xbox360 { get; set; }
        [Inject] protected XboxOneSpriteConfig XboxOne { get; set; }
        [Inject] protected MainResourceLoader MainResourceLoader { get; set; }

        [OnReady("AnimationPlayer")] private AnimationPlayer _animation;

        public override void _Ready() {
            if (_config == null) {
                // TODO: this should depends on the controller connected
                Configure(XboxOne);
            }
        }
        
        public void Configure(SpriteConfig config) {
            _config = config;
            Texture = _config.Texture;
        }

        public enum State {
            Normal,
            Pressed,
            Animated
        }

        public string? Animation = null;
        public JoystickList Button = JoystickList.InvalidOption;
        public bool IsAxis = false;
        private State _state = State.Normal;

        public void SetButton(bool isAxis, JoystickList button, State state) {
            Animation = null;
            IsAxis = isAxis;
            Button = button;
            _state = state;

            if (state == State.Animated) {
                var buttonAnimation = _config.Get(button, isAxis)?.Animation;
                if (buttonAnimation != null && _animation.HasAnimation(buttonAnimation)) {
                    Animation = buttonAnimation;
                } else {
                    _state = State.Normal;
                }
            }            
            Refresh();
        }

        public void SetAxis(JoystickList button, bool animate) {
            SetButton(true, button, animate ? State.Animated : State.Normal);
        }

        public void SetButton(JoystickList button, bool pressed) {
            SetButton(false, button, pressed ? State.Pressed : State.Normal);
        }

        public void SetPressed(bool pressed) {
            _state = pressed ? State.Pressed : State.Normal;
            Refresh();
        }

        public void Animate(string animation) {
            Animation = animation;
            _state = State.Animated;
            Refresh();
        }
        
        
        public bool HasAnimation(string animation) {
            return _animation.HasAnimation(animation);
        }

        public void InputAction(bool isAxis, InputAction action, State state) {
            if (isAxis) {
                SetButton(true, (JoystickList)action.Axis, state);
            } else {
                var buttons = action.Buttons;
                if (buttons.Count > 0) SetButton(false, buttons[0], state);
            }
        }

        private void Refresh() {
            if (_state == State.Animated && Animation != null) {
                _animation.Play(Animation);
            } else {
                _animation.Stop();
                ConsoleButtonView? view = _config.Get(Button, IsAxis);
                Frame = view != null ? _state == State.Pressed ? view.FramePressed : view.Frame : 0;
            }
        }
    }

    public abstract class SpriteConfig {
        private readonly Dictionary<int, ConsoleButtonView> _mapping = new();

        private readonly ConsoleButtonView _default;

        public SpriteConfig() {
            _default = CreateDefaultView();
            ConfigureButtons();
        }
        
        private const int AxisOffset = 1000;
        
        public abstract Texture Texture { get; }

        public int GetFrame(JoystickList button, bool isAxis) => Get(button, isAxis)?.Frame ?? 0;
        public int GetFramePressed(JoystickList button, bool isAxis) => Get(button, isAxis)?.FramePressed ?? 0;

        public ConsoleButtonView? Get(JoystickList button, bool isAxis) =>
            _mapping.TryGetValue((int)button + (isAxis ? AxisOffset : 0), out var o) ? o : null;

        public abstract void ConfigureButtons();
        public abstract ConsoleButtonView CreateDefaultView();

        protected void Button(JoystickList joystickList, string animation, int frame, int framePressed) =>
            Add((int)joystickList, animation, frame, framePressed);
        
        protected void Axis(int axis, string animation, int frame, int framePressed) =>
            Add(axis + AxisOffset, animation, frame, framePressed);
        
        private void Add(int x, string animation, int frame, int framePressed) =>
            _mapping.Add(x, new ConsoleButtonView(animation, frame, framePressed));
    }

    [Service]
    public class Xbox360SpriteConfig : SpriteConfig {
        public override ConsoleButtonView CreateDefaultView() => new(null, 0, 0);

        [Inject] private MainResourceLoader _mainResourceLoader { get; set; }
        public override Texture Texture => _mainResourceLoader.Xbox360ButtonsTexture;

        public override void ConfigureButtons() {
            Button(JoystickList.XboxA, "A", 13, 14);
            Button(JoystickList.XboxB, "B", 49, 50);
            Button(JoystickList.XboxX, "X", 25, 26);
            Button(JoystickList.XboxY, "Y", 37, 38);

            Button(JoystickList.L, null, 46, 45); // LB
            Button(JoystickList.R, null, 58, 57); // RB
            Button(JoystickList.AnalogL2, null, 22, 21); // LT
            Button(JoystickList.AnalogR2, null, 34, 33); // RT

            Button(JoystickList.Select, "", 16, 15);
            Button(JoystickList.Start, "", 19, 20);
            Button(JoystickList.Guide, "", 17, 18); // Xbox Button (big button between select & start)

            Button(JoystickList.DpadRight, "", 28, 27);
            Button(JoystickList.DpadDown, "", 29, 27);
            Button(JoystickList.DpadLeft, "", 30, 27);
            Button(JoystickList.DpadUp, "", 31, 27);

            // Right analog Click
            Button(JoystickList.R3, "", 39, 44);

            // Right analog Click
            Button(JoystickList.L3, "left click", 51, 56);


            // Right analog stick:
            // 39:Stop, 40:R, 41:D, 42:L, 43:U
            // [40, 41, 42, 43] Circle
            // [39, 40, 39, 42] Right & left
            // [39, 43, 39, 41] Up & down
            
            // Left analog stick:
            // 51:Stop, 52:R, 53:D, 54:L, 55:U, 56:pressed
            Axis(0, "left lateral", 51, 51); // No pressed, axis!
            // [52, 53, 54, 55] Circle
            // [51, 52, 51, 54] Right & left
            // [51, 55, 51, 53] Up & down

            // Analog stick (without R/L)
            // 63:Stop, 64:R, 65:D, 66:L, 67:U

        }
    }

    [Service]
    public class XboxOneSpriteConfig : Xbox360SpriteConfig {
        [Inject] private MainResourceLoader _mainResourceLoader { get; set; }
        public override Texture Texture => _mainResourceLoader.XboxOneButtonsTexture;
    }

    public class ConsoleButtonView {
        public readonly int Frame;
        public readonly int FramePressed;
        public readonly string? Animation;

        public ConsoleButtonView(string? animation, int frame, int framePressed) {
            Animation = animation;
            Frame = frame;
            FramePressed = framePressed;
        }

    }
}