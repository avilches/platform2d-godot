using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;

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
        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;


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

        public MainMenuBottomBar AddButton(string? label1, string animation, string? label2) {
            ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible);
            hint.Labels(label1, label2).Button(animation, true);
            hint.Visible = true;
            return this;
        }

        public MainMenuBottomBar AddButton(string? label1, ActionState action, string? label2, bool animate = false) {
            ActionHint hint = _actionHintList.Find(actionHint => !actionHint.Visible);
            hint.Labels(label1, label2).Button(action, animate);
            hint.Visible = true;
            return this;
        }

        // TODO: i18n
        public void ConfigureMenuAcceptBack() {
            HideAll()
                .AddButton(null,UiAccept, "Accept")
                .AddButton(null,UiCancel, "Back");
        }

        public void ConfigureModalAcceptCancel() {
            HideAll()
                .AddButton(null,UiAccept, "Accept")
                .AddButton(null,UiCancel, "Cancel");
        }

        public void ConfigureSettingsChangeBack() {
            HideAll()
                .AddButton(null,UiAccept, "Change")
                .AddButton(null,UiCancel, "Back");
        }

        public void ConfigureSettingsResolution() {
            HideAll()
                .AddButton(null,"left lateral", "Change resolution")
                .AddButton(null,UiCancel, "Back");
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