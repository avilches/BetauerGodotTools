using System.Collections.Generic;
using Betauer.DI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public abstract class ConsoleButton : DiSprite {
        private SpriteConfig _config;

        [Inject] protected Xbox360SpriteConfig Xbox360;
        [Inject] protected XboxOneSpriteConfig XboxOne;
        
        [Inject] protected ResourceManager _resourceManager;

        public override void Ready() {
            if (_config == null) {
                // TODO: this should depends on the controller connected
                Configure(Xbox360);
            }
        }
        
        public void Configure(SpriteConfig config) {
            _config = config;
            Texture = _resourceManager.Resource<Texture>(_config.Texture);
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
            ConsoleButtonView view = _config.Get(button);
            Frame = pressed ? view.FramePressed : view.Frame;
        }
    }

    public abstract class SpriteConfig {
        private readonly Dictionary<JoystickList, ConsoleButtonView> _mapping =
            new Dictionary<JoystickList, ConsoleButtonView>();

        private readonly ConsoleButtonView _default;

        public SpriteConfig() {
            _default = CreateDefaultView();
            ConfigureButtons();
            Texture = ConfigureTexture();
        }

        public readonly string Texture;

        public int GetFrame(JoystickList button) => Get(button).Frame;
        public int GetFramePressed(JoystickList button) => Get(button).FramePressed;

        public ConsoleButtonView Get(JoystickList button) => _mapping.TryGetValue(button, out var o) ? o : _default;

        public abstract void ConfigureButtons();
        public abstract ConsoleButtonView CreateDefaultView();
        protected abstract string ConfigureTexture();

        protected void Add(JoystickList joystickList, int frame, int framePressed) {
            _mapping.Add(joystickList, new ConsoleButtonView(frame, framePressed));
        }
    }

    [Singleton]
    public class Xbox360SpriteConfig : SpriteConfig {
        public override ConsoleButtonView CreateDefaultView() => new ConsoleButtonView(0, 0);
        
        protected override string ConfigureTexture() {
            return ResourceManager.Xbox360Buttons;
        }

        public override void ConfigureButtons() {
            Add(JoystickList.XboxA, 13, 14);
            Add(JoystickList.XboxB, 49, 50);
            Add(JoystickList.XboxX, 25, 26);
            Add(JoystickList.XboxY, 37, 38);

            Add(JoystickList.L, 46, 45); // LB
            Add(JoystickList.R, 58, 57); // RB
            Add(JoystickList.AnalogL2, 22, 21); // LT
            Add(JoystickList.AnalogR2, 34, 33); // RT

            Add(JoystickList.Select, 16, 15);
            Add(JoystickList.Start, 19, 20);
            Add(JoystickList.Guide, 17, 18); // Xbox Button (big button between select & start)

            Add(JoystickList.DpadRight, 27, 28);
            Add(JoystickList.DpadDown, 27, 29);
            Add(JoystickList.DpadLeft, 27, 30);
            Add(JoystickList.DpadUp, 27, 31);

            Add(JoystickList.R3, 39, 44);
            Add(JoystickList.L3, 51, 56);


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

    [Singleton]
    public class XboxOneSpriteConfig : Xbox360SpriteConfig {
        protected override string ConfigureTexture() {
            return ResourceManager.XboxOneButtons;
        }

    }

    public struct ConsoleButtonView {
        public readonly int Frame;
        public readonly int FramePressed;

        public ConsoleButtonView(int frame, int framePressed) {
            Frame = frame;
            FramePressed = framePressed;
        }
    }
}