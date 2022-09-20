using System.Collections.Generic;
using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.StateMachine;
using Godot;
using Veronenger.Game.Controller.UI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenuBottomBar : CanvasLayer, IStateMachineListener<GameManager.State> {
        [OnReady("%ActionHint1")] private ActionHint _actionHint1;
        [OnReady("%ActionHint2")] private ActionHint _actionHint2;
        [OnReady("%ActionHint3")] private ActionHint _actionHint3;
        [OnReady("%ActionHint4")] private ActionHint _actionHint4;
        private readonly List<ActionHint> _actionHintList = new List<ActionHint>();
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }
        [Inject] private InputAction UiLeft { get; set; }

                    
        public override void _Ready() {
            _actionHintList.Add(_actionHint1);
            _actionHintList.Add(_actionHint2);
            _actionHintList.Add(_actionHint3);
            _actionHintList.Add(_actionHint4);
        }

        public MainMenuBottomBar HideAll() {
            _actionHint1.Visible = _actionHint2.Visible = _actionHint3.Visible = _actionHint4.Visible = false;
            return this;
        }

        public MainMenuBottomBar AddButton(string? label1, InputAction inputAction, bool isAxis, string? label2,
            bool animate = true) {
            ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible);
            hint.Labels(label1, label2).InputAction(inputAction, isAxis, animate);
            hint.Visible = true;
            return this;
        }

        // TODO: i18n
        public void ConfigureMenuAcceptBack() {
            HideAll()
                .AddButton(null, UiAccept, false, "Accept")
                .AddButton(null, UiCancel, false, "Back");
        }

        public void ConfigureModalAcceptCancel() {
            HideAll()
                .AddButton(null, UiAccept, false, "Accept")
                .AddButton(null, UiCancel, false, "Cancel");
        }

        public void ConfigureSettingsChangeBack() {
            HideAll()
                .AddButton(null, UiAccept, false, "Change")
                .AddButton(null, UiCancel, false, "Back");
        }

        public void ConfigureSettingsResolution() {
            HideAll()
                .AddButton(null, UiLeft, true, "Change")
                .AddButton(null, UiCancel, false, "Back");
        }

        public void OnEnter(GameManager.State state, GameManager.State from) {
        }

        public void OnAwake(GameManager.State state, GameManager.State from) {
        }

        public void OnSuspend(GameManager.State state, GameManager.State to) {
        }

        public void OnExit(GameManager.State state, GameManager.State to) {
        }

        public void OnExecuteStart(float delta, GameManager.State state) {
        }

        public void OnExecuteEnd(GameManager.State state) {
        }

        public void OnTransition(GameManager.State from, GameManager.State to) {
            switch (to) {
                case GameManager.State.ModalExitDesktop:
                case GameManager.State.ModalQuitGame:
                    ConfigureModalAcceptCancel();
                    break;
                case GameManager.State.MainMenu:
                case GameManager.State.PauseMenu:
                    ConfigureMenuAcceptBack();
                    break;
                case GameManager.State.Gaming:
                case GameManager.State.ExitDesktop:
                    HideAll();
                    break;
            }
        }
    }
}