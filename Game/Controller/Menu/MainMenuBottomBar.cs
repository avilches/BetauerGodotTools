using System.Collections.Generic;
using Betauer.DI;
using Betauer.StateMachine;
using Veronenger.Game.Controller.UI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenuBottomBar: DiCanvasLayer, IStateMachineListener<GameManager.State> {
        [OnReady("HBoxContainer/ActionHint1")] private ActionHint _actionHint1; 
        [OnReady("HBoxContainer/ActionHint2")] private ActionHint _actionHint2; 
        [OnReady("HBoxContainer/ActionHint3")] private ActionHint _actionHint3; 
        [OnReady("HBoxContainer/ActionHint4")] private ActionHint _actionHint4;
        private readonly List<ActionHint> _actionHintList = new List<ActionHint>();

        public override void Ready() {
            _actionHintList.Add(_actionHint1);
            _actionHintList.Add(_actionHint2);
            _actionHintList.Add(_actionHint3);
            _actionHintList.Add(_actionHint4);
        }

        public MainMenuBottomBar HideAll() {
            _actionHint1.Visible = _actionHint2.Visible = _actionHint3.Visible = _actionHint4.Visible = false;
            return this;
        }

        public MainMenuBottomBar AddButton(string? label1, string action, string? label2) {
            ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible);
            hint.Configure(label1, action, label2);
            hint.Visible = true;
            return this;
        }

        public void Save() {
            _actionHint1.Save();
            _actionHint2.Save();
            _actionHint3.Save();
            _actionHint4.Save();
        }

        public void Restore() {
            _actionHint1.Restore();
            _actionHint2.Restore();
            _actionHint3.Restore();
            _actionHint4.Restore();
        }

        // TODO: i18n
        public void ConfigureAcceptBack() {
            HideAll()
                .AddButton(null,"ui_accept", "Accept")
                .AddButton(null,"ui_cancel", "Back");
        }

        public void ConfigureAcceptCancel() {
            HideAll()
                .AddButton(null,"ui_accept", "Accept")
                .AddButton(null,"ui_cancel", "Cancel");
        }

        public void OnEnter(GameManager.State state, GameManager.State from) {
        }

        public void OnAwake(GameManager.State state, GameManager.State from) {
        }

        public void OnSuspend(GameManager.State state, GameManager.State to) {
        }

        public void OnExit(GameManager.State state, GameManager.State to) {
        }

        public void OnTransition(GameManager.State from, GameManager.State to) {
            switch (to) {
                case GameManager.State.ModalExitDesktop:
                case GameManager.State.ModalQuitGame:
                    ConfigureAcceptCancel();
                    break;
                case GameManager.State.MainMenu:
                case GameManager.State.PauseMenu:
                    ConfigureAcceptBack();
                    break;
                case GameManager.State.Gaming:
                case GameManager.State.ExitDesktop:
                    HideAll();
                    break;
            }
        }
    }
}