using Betauer.Application.Monitor;
using Godot;
using Betauer.Application.Screen;
using Betauer.Bus;
using Betauer.Core;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.Nodes;
using Betauer.StateMachine.Async;
using Veronenger.UI;

namespace Veronenger.Managers; 

public enum MainState {
    Init,
    MainMenu,
    Settings,
    StartingGame,
    Gaming,
    GameOver,
    PauseMenu,
    ModalQuitGame,
    QuitGame,
    ModalExitDesktop,
    ExitDesktop,
}
    
public enum MainEvent {
    Back,
    Pause,
    Settings,
    StartGame,
    EndGame,
    ModalBoxConfirmExitDesktop,
    ModalBoxConfirmQuitGame,
    ExitDesktop
}
    
[Service]
public partial class MainStateMachine : StateMachineNodeAsync<MainState, MainEvent>, IInjectable {

    [Inject] private MainMenu MainMenuScene { get; set; }
    [Inject] public BottomBar BottomBarScene { get; set; }
    [Inject] private PauseMenu PauseMenuScene { get; set; }
    [Inject] private SettingsMenu SettingsMenuScene { get; set; }
    [Inject] private HUD HudScene { get; set; }
    [Inject] private IFactory<ModalBoxConfirm> ModalBoxConfirm { get; set; }
    [Inject] private Theme MyTheme { get; set; }
    [Inject] private Game Game { get; set; }

    [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

    [Inject] private InputAction UiAccept { get; set; }
    [Inject] private InputAction UiCancel { get; set; }
    [Inject] private InputAction ControllerStart { get; set; }
        
    [Inject] private Theme DebugConsoleTheme { get; set; }

    [Inject] private EventBus EventBus { get; set; }

    public override void _Ready() {
        ProcessMode = ProcessModeEnum.Always;
    }

    public MainStateMachine() : base(MainState.Init) {
    }

    public void PostInject() {
#if DEBUG
        this.OnInput((e) => {
            if (e.IsKeyPressed(Key.Q)) {
                // _settingsMenuScene.Scale -= new Vector2(0.05f, 0.05f);
                // Engine.TimeScale -= 0.05f;
            }
            if (e.IsKeyPressed(Key.W)) {
                // _settingsMenuScene.Scale = new Vector2(1, 1);
                // Engine.TimeScale = 1;
            }
            if (e.IsKeyPressed(Key.E)) {
                // Engine.TimeScale += 0.05f;
                // _settingsMenuScene.Scale += new Vector2(0.05f, 0.05f);
            }
        }, ProcessModeEnum.Always);
#endif
            
        EventBus.Subscribe(evt => Send(evt)).UnsubscribeIf(Predicates.IsInvalid(this));
        var modalResponse = false;
        var splashScreen = SceneTree.GetMainScene<SplashScreenNode>();
        splashScreen.Layer = int.MaxValue;

        var endSplash = false;

        On(MainEvent.ModalBoxConfirmExitDesktop).Push(MainState.ModalExitDesktop);
        On(MainEvent.ExitDesktop).Set(MainState.ExitDesktop);
        On(MainEvent.ModalBoxConfirmQuitGame).Push(MainState.ModalQuitGame);
        OnTransition += args => BottomBarScene.UpdateState(args.To);
        State(MainState.Init)
            .Enter(() => {
                ConfigureCanvasLayers();
                ConfigureDebugOverlays();
                ScreenSettingsManager.Setup();
            })
            .OnInput(e => {
                if (!endSplash && (e.IsAnyKey() || e.IsAnyButton() || e.IsAnyClick()) && e.IsJustPressed()) {
                    splashScreen.QueueFree();
                    endSplash = true;
                }
            })
            .If(() => endSplash).Set(MainState.MainMenu)
            .Build();
            
        State(MainState.MainMenu)
            .OnInput(e => MainMenuScene.OnInput(e))
            .On(MainEvent.StartGame).Set(MainState.StartingGame)
            .On(MainEvent.Settings).Push(MainState.Settings)
            .Suspend(() => MainMenuScene.DisableMenus())
            .Awake(() => MainMenuScene.EnableMenus())
            .Enter(async () => await MainMenuScene.ShowMenu())
            .Build();

        State(MainState.Settings)
            .OnInput(SettingsMenuScene.OnInput)
            .On(MainEvent.Back).Pop()
            .Enter(() => SettingsMenuScene.ShowSettingsMenu())
            .Exit(() => SettingsMenuScene.HideSettingsMenu())
            .Build();

        State(MainState.StartingGame)
            .Enter(async () => {
                await MainMenuScene.HideMainMenu();
                await Game.Start();
            })
            .If(() => true).Set(MainState.Gaming)
            .Build();

        State(MainState.Gaming)
            .On(MainEvent.EndGame).Set(MainState.GameOver)
            .OnInput(e => {
                if (ControllerStart.IsEventJustPressed(e)) {
                    Send(MainEvent.Pause);
                    GetViewport().SetInputAsHandled();
                }
            })
            .On(MainEvent.Back).Pop()
            .On(MainEvent.Pause).Push(MainState.PauseMenu)
            .Build();

        State(MainState.GameOver)
            .Enter(() => Game.End())
            .If(() => true).Set(MainState.MainMenu)
            .Build();
            
        State(MainState.PauseMenu)
            .OnInput(PauseMenuScene.OnInput)
            .On(MainEvent.Back).Pop()
            .On(MainEvent.Settings).Push(MainState.Settings)
            .Suspend(() => PauseMenuScene.DisableMenus())
            .Awake(() => PauseMenuScene.EnableMenus())
            .Enter(async () => {
                SceneTree.Paused = true;
                await PauseMenuScene.ShowPauseMenu();
            })
            .Exit(() => {
                PauseMenuScene.EnableMenus();
                PauseMenuScene.HidePauseMenu();
                SceneTree.Paused = false;
            })
            .Build();

        State(MainState.ModalQuitGame)
            .On(MainEvent.Back).Pop()
            .Execute(async () => {
                var modalBoxConfirm = ShowModalBox("Quit game?", "Any progress not saved will be lost");
                modalResponse = await modalBoxConfirm.AwaitResult();
            })
            .If(() => modalResponse).Set(MainState.QuitGame)
            .If(() => !modalResponse).Pop()
            .Build();
                
        State(MainState.QuitGame)
            .Enter(() => Game.End())
            .If(() => true).Set(MainState.MainMenu)
            .Build();
                

        State(MainState.ModalExitDesktop)
            .On(MainEvent.Back).Pop()
            .Execute(async () => {
                var modalBoxConfirm = ShowModalBox("Exit game?");
                modalBoxConfirm.FadeBackgroundOut(1, 0.5f);
                modalResponse = await modalBoxConfirm.AwaitResult();
            })
            .If(() => modalResponse).Set(MainState.ExitDesktop)
            .If(() => !modalResponse).Pop()
            .Build();
                
                
        State(MainState.ExitDesktop)
            .Enter(() => SceneTree.QuitSafely())
            .Build();
    }

    private void ConfigureCanvasLayers() {
        MainMenuScene.Layer = CanvasLayerConstants.MainMenu;
        PauseMenuScene.Layer = CanvasLayerConstants.PauseMenu;
        SettingsMenuScene.Layer = CanvasLayerConstants.SettingsMenu;
        BottomBarScene.Layer = CanvasLayerConstants.BottomBar;
        HudScene.Layer = CanvasLayerConstants.HudScene;
        HudScene.Visible = false;

        OnlyInPause(PauseMenuScene);
        NeverPause(SettingsMenuScene, BottomBarScene);
    }

    private void NeverPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.Always);
    private void OnlyInPause(params Node[] nodes) => nodes.ForEach(n=> n.ProcessMode = ProcessModeEnum.WhenPaused);

    private void ConfigureDebugOverlays() {
        DebugOverlayManager.OverlayContainer.Theme = MyTheme;
        DebugOverlayManager.DebugConsole.Theme =  DebugConsoleTheme;;
    }

    private ModalBoxConfirm ShowModalBox(string title, string subtitle = null) {
        var modalBoxConfirm = ModalBoxConfirm.Get();
        modalBoxConfirm.Layer = CanvasLayerConstants.ModalBox;
        modalBoxConfirm.Title(title, subtitle);
        modalBoxConfirm.ProcessMode = ProcessModeEnum.Always;
        SceneTree.Root.AddChild(modalBoxConfirm);
        modalBoxConfirm.AwaitResult().ContinueWith(task => modalBoxConfirm.QueueFree());
        return modalBoxConfirm;
    }
}