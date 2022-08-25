using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI.Consoles {
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

        public JoystickList _buttonToShow = JoystickList.InvalidOption;
        public bool _pressed = false;

        public void ShowButton(JoystickList buttonToShow) {
            _buttonToShow = buttonToShow;
            Change(_buttonToShow, _pressed);
        }

        public void ShowButton(JoystickList buttonToShow, bool pressed) {
            _buttonToShow = buttonToShow;
            _pressed = pressed;
            Change(_buttonToShow, _pressed);
        }

        public void ShowPressed(bool pressed) {
            _pressed = pressed;
            Change(_buttonToShow, _pressed);
        }

        public void Change(JoystickList button, bool pressed) {
            _animation.Stop();
            ConsoleButtonView view = _config.Get(button);
            Frame = pressed ? view.FramePressed : view.Frame;
        }

        public void AnimateLeftVertical() {
            _animation.Play("left vertical");
        }

        public void AnimateLeftLateral() {
            _animation.Play("left lateral");
        }

        public void AnimateLeftCircular() {
            _animation.Play("left circular");
        }

        public bool HasAnimation(string animation) {
            return _animation.HasAnimation(animation);
        }

        public void Animate(string animation) {
            _animation.Play(animation);
        }

        public void Animate(JoystickList button) {
            ConsoleButtonView view = _config.Get(button);
            if (!string.IsNullOrEmpty(view.Animation) && _animation.HasAnimation(view.Animation)) {
                _animation.Play(view.Animation);
            } else {
                ShowButton(button);
            }
        }
        
        public void InputAction(InputAction action, bool animate = false) {
            if (action.Buttons.Count > 0) {
                JoystickList button = action.Buttons.First();
                if (animate) {
                    Animate(button);
                } else {
                    ShowButton(button);
                }
            }
        }
        
    }

    public abstract class SpriteConfig {
        private readonly Dictionary<JoystickList, ConsoleButtonView> _mapping =
            new Dictionary<JoystickList, ConsoleButtonView>();

        private readonly ConsoleButtonView _default;

        public SpriteConfig() {
            _default = CreateDefaultView();
            ConfigureButtons();
        }
        
        public abstract Texture Texture { get; }

        public int GetFrame(JoystickList button) => Get(button).Frame;
        public int GetFramePressed(JoystickList button) => Get(button).FramePressed;

        public ConsoleButtonView Get(JoystickList button) => _mapping.TryGetValue(button, out var o) ? o : _default;

        public abstract void ConfigureButtons();
        public abstract ConsoleButtonView CreateDefaultView();

        protected void Add(JoystickList joystickList, string animation, int frame, int framePressed) {
            _mapping.Add(joystickList, new ConsoleButtonView(animation, frame, framePressed));
        }
    }

    [Service]
    public class Xbox360SpriteConfig : SpriteConfig {
        public override ConsoleButtonView CreateDefaultView() => new ConsoleButtonView(null, 0, 0);

        [Inject] private MainResourceLoader _mainResourceLoader { get; set; }
        public override Texture Texture => _mainResourceLoader.Xbox360ButtonsTexture;

        public override void ConfigureButtons() {
            Add(JoystickList.XboxA, "A", 13, 14);
            Add(JoystickList.XboxB, "B", 49, 50);
            Add(JoystickList.XboxX, "X", 25, 26);
            Add(JoystickList.XboxY, "Y", 37, 38);

            Add(JoystickList.L, null, 46, 45); // LB
            Add(JoystickList.R, null, 58, 57); // RB
            Add(JoystickList.AnalogL2, null, 22, 21); // LT
            Add(JoystickList.AnalogR2, null, 34, 33); // RT

            Add(JoystickList.Select, "", 16, 15);
            Add(JoystickList.Start, "", 19, 20);
            Add(JoystickList.Guide, "", 17, 18); // Xbox Button (big button between select & start)

            Add(JoystickList.DpadRight, "", 28, 27);
            Add(JoystickList.DpadDown, "", 29, 27);
            Add(JoystickList.DpadLeft, "", 30, 27);
            Add(JoystickList.DpadUp, "", 31, 27);

            // Right analog Click
            Add(JoystickList.R3, "", 39, 44);

            // Right analog Click
            Add(JoystickList.L3, "left click", 51, 56);


            // Right analog stick:
            // 39:Stop, 40:R, 41:D, 42:L, 43:U
            // [40, 41, 42, 43] Circle
            // [39, 40, 39, 42] Right & left
            // [39, 43, 39, 41] Up & down
            
            // Left analog stick:
            // 51:Stop, 52:R, 53:D, 54:L, 55:U
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

    public struct ConsoleButtonView {
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