using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;

using Betauer.StateMachine;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;
using Container = Godot.Container;

namespace Veronenger.Game.Controller.Menu {
    public class PauseMenu : CanvasLayer {
        private static readonly SequenceTemplate PartialFadeOut = TemplateBuilder.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Property.Opacity)
            .KeyframeTo(0f, 0f)
            .KeyframeTo(1f, 0.8f)
            .EndAnimate()
            .BuildTemplate();

        [OnReady("Node2D")] private Node2D _container;

        [OnReady("Node2D/BackgroundFader")] private ColorRect _backgroundFader;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Menu")]
        private Container _menuBase;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Title")]
        private Label _title;

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);
            _menuController = BuildMenu();
            HidePauseMenu();
        }

        public async Task ShowPauseMenu() {
            _container.Show();
            _launcher.Play(PartialFadeOut, _backgroundFader, 0f, 0.5f);
            await _menuController.Start("Root");
        }

        public void HidePauseMenu() {
            _launcher.RemoveAll();
            _container.Hide();
        }

        public void DisableMenus() {
            _menuController.ActiveMenu!.Save();
            _menuController.ActiveMenu!.DisableButtons();
        }

        public void EnableMenus() {
            _menuController.ActiveMenu!.Restore();
        }

        public MenuController BuildMenu() {
            // TODO i18n
            _title.Text = "Paused";
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("Resume", "Resume", (ctx) => _gameManager.TriggerBack())
                .AddButton("Settings", "Settings", (ctx) => _gameManager.TriggerSettings())
                .AddButton("QuitGame", "Quit game", (ctx) => _gameManager.TriggerModalBoxConfirmQuitGame());
            return mainMenu;
        }

        public bool IsRootMenuActive() {
            return _menuController.ActiveMenu?.Name == "Root";
        }

        public async Task BackMenu() {
            await _menuController.Back(BackGoodbyeAnimation, BackNewMenuAnimation);
        }

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackOutLeftFactory.Get(150), transition.FromMenu.Control, 0f, MenuEffectTime).Await();
            // await _launcher.Play(Template.FadeOut, transition.FromButton, 0f, MenuEffectTime*2).Await();
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.FromMenu.GetChildren()) {
                if (child is Control control) {
                    // actionButton.Modulate =
                    // new Color(actionButton.Modulate.r, actionButton.Modulate.g, actionButton.Modulate.b, 0);
                    lastToWaitFor = _launcher.Play(Template.FadeOutLeft, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.FadeOutDown, transition.FromMenu.CanvasItem, 0f, 0.25f).Await();
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            int x = 0;
            LoopStatus lastToWaitFor = null;
            foreach (var child in transition.ToMenu.GetChildren()) {
                if (child is Control control) {
                    control.Modulate = new Color(1f, 1f, 1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeInRight, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackInRightFactory.Get(200), _menuHolder, 0f, MenuEffectTime).Await();
        }


        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.FromMenu.GetChildren()) {
                if (child is Control control) {
                    // control.Modulate = new Color(1f,1f,1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeOutRight, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackOutRightFactory.Get(200), transition.FromMenu.CanvasItem, 0f,
            // MenuEffectTime)
            // .Await();
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackInLeftFactory.Get(150), transition.ToMenu.CanvasItem, 0f,
            // MenuEffectTime)
            // .Await();
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.ToMenu.GetChildren()) {
                if (child is Control control) {
                    control.Modulate = new Color(1f, 1f, 1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeInLeft, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
        }

        private const float MenuEffectTime = 0.10f;

    }
}