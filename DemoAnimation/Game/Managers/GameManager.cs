using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Signal;
using Betauer.StateMachine;
using DemoAnimation.Game.Controller.Menu;
using Godot;

namespace DemoAnimation.Game.Managers {
    [Service]
    public class GameManager : StateMachineNode<GameManager.State, GameManager.Transition> {
        public enum Transition {
            FinishLoading,
            Back,
            ModalBoxConfirmExitDesktop,
        }
    
        public enum State {
            Init,
            MainMenu,
            ModalExitDesktop,
            ExitDesktop,
        }
    
        private Node _currentGameScene;

        [Load("res://Assets/UI/my_theme.tres")]
        private Theme MyTheme;

        [Load("res://Scenes/Menu/MainMenu.tscn")]
        private MainMenu _mainMenuScene;

        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }
        [Inject] private DebugOverlay DebugOverlay { get; set; }
        [Inject] private SceneTree _sceneTree { get; set; }
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        public GameManager() : base(State.Init) {
            On(Transition.FinishLoading, context => context.PopPush(State.MainMenu));
            CreateState(State.Init)
                .Enter(async () => {
                    await new ResourceLoaderContainer().From(this).Load();
                    _screenSettingsManager.Setup();
                    _sceneTree.Root.AddChild(_mainMenuScene);

                    DebugOverlay.Panel.Theme = MyTheme;
                    DebugOverlay.DebugOverlayAction = InputAction.Create("Debug").Keys(KeyList.F9).Build();
                    DebugOverlay.DebugOverlayAction.Setup();
                    DebugOverlay.AddFpsAndMemory();
                    DebugOverlay.AddObjectRunnerSize();
                    DebugOverlay.Create().WithPrefix("SceneTreeTween callbacks").Show(() => DefaultTweenCallbackManager.Instance.ActionsByTween.Count.ToString());
                })
                .Execute(context => context.Set(State.MainMenu))
                .Build();
                

            CreateState(State.MainMenu)
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(async context => {
                    if (UiCancel.IsJustPressed()) {
                        await _mainMenuScene.BackMenu();
                    }
                    return context.None();
                })
                .Build();
                

            On(Transition.ModalBoxConfirmExitDesktop, context => context.Push(State.ModalExitDesktop));
            CreateState(State.ModalExitDesktop)
                .On(Transition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Push(State.ExitDesktop) : context.Pop();
                })
                .Build();

            CreateState(State.ExitDesktop)
                .Enter(() => _sceneTree.Notification(MainLoop.NotificationWmQuitRequest))
                .Build();
            
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            Enqueue(Transition.ModalBoxConfirmExitDesktop);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm =
                (ModalBoxConfirm)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/ModalBoxConfirm.tscn").Instance();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            _sceneTree.Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }
        private async Task AddSceneDeferred(Node scene) {
            await _sceneTree.AwaitIdleFrame();
            _sceneTree.Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);
        }

    }
}