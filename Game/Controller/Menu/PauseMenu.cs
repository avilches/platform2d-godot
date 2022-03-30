using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;
using Container = Godot.Container;

namespace Veronenger.Game.Controller.Menu {
    public class PauseMenu : DiCanvasLayer {
        private static readonly SequenceTemplate PartialFadeOut = TemplateBuilder.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Property.Opacity)
            .KeyframeTo(0f, 0f)
            .KeyframeTo(1f, 0.8f)
            .EndAnimate()
            .BuildTemplate();

        [OnReady("Panel")]
        private Panel _container;

        [OnReady("ColorRect")] private ColorRect _colorRect;

        [OnReady("Panel/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("Panel/VBoxContainer/Title")]
        private Label _title;

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiAccept => _inputManager.UiAccept;
        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionState UiStart => _inputManager.UiStart;
        private ActionButton _optionsButton;

        private Launcher _launcher;

        public override void Ready() {
            _launcher = new Launcher().WithParent(this);
            _menuController = BuildMenu();
            _optionsButton = _menuController.GetMenu("Root")!.GetButton("Options");
            Hide();
        }

        public async Task Show() {
            _container.Visible = _colorRect.Visible = true;
            _launcher.Play(PartialFadeOut, _colorRect, 0f, 0.5f);
            await _menuController.Start("Root");
        }

        public void FocusOptions() {
            _optionsButton.GrabFocus();
        }

        public void Hide() {
            _launcher.RemoveAll();
            _container.Visible = _colorRect.Visible = false;
        }

        public MenuController BuildMenu() {
            // TODO i18n
            _title.Text = "Paused";
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("Resume", "Resume", (ctx) => {
                    _gameManager.ClosePauseMenu();
                })
                .AddButton("Options", "Options",
                    (ctx) => _gameManager.ShowOptionsMenu())
                .AddButton("QuitGame", "Quit game", async (ctx) => {
                    var result = await _gameManager.ModalBoxConfirmQuitGame();
                    if (result) {
                        _gameManager.ClosePauseMenu();
                        await _gameManager.ExitGameAndBackToMainMenu();
                    } else {
                        ctx.ActionButton.GrabFocus();
                    }
                });

            return mainMenu;
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

        public override void _Input(InputEvent @event) {
            if (!_gameManager.IsGamePaused()) {
                return;
            }
            if (UiCancel.IsEventPressed(@event)) {
                if (_menuController.ActiveMenu?.Name == "Root") {
                    _gameManager.ClosePauseMenu();
                    GetTree().SetInputAsHandled();
                } else {
                    _menuController.Back(BackGoodbyeAnimation, BackNewMenuAnimation);
                }
            } else if (UiStart.IsEventPressed(@event)) {
                _gameManager.ClosePauseMenu();
                GetTree().SetInputAsHandled();
            }
        }
    }
}