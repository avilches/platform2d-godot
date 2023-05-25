using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Application.Settings.Attributes;
using Betauer.Camera;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Betauer.Input.Joypad;
using Godot;
using Pcg;
using Veronenger.Character.Npc;
using Veronenger.Character.Player;
using Veronenger.Managers;
using Veronenger.Transient;
using Veronenger.UI;
using Veronenger.Worlds;

namespace Veronenger.Config; 

[Configuration]
public class ApplicationConfig {
	public static readonly ScreenConfiguration Configuration = new(
		FixedViewportStrategy.Instance, 
		Resolutions.FULLHD_DIV2,
		Resolutions.FULLHD_DIV2,
		Window.ContentScaleModeEnum.CanvasItems, // (viewport is blur)
		Window.ContentScaleAspectEnum.Keep,
		Resolutions.GetAll(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9), 
		true,
		1f);

	[Singleton] public JsonGameLoader<MySaveGame> GameObjectLoader() {
		var loader = new JsonGameLoader<MySaveGame>();
		loader.WithJsonSerializerOptions(options => {
			options.AllowTrailingCommas = true;
			options.WriteIndented = true;
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		});
		return loader;
	}
	[Singleton] public Random Random => new PcgRandom();
	[Singleton] public DebugOverlayManager DebugOverlayManager => new();
	[Singleton] public GameObjectRepository GameObjectRepository => new();
	[Singleton] public UiActionsContainer UiActionsContainer => new();
	[Singleton] public InputActionsContainer PlayerActionsContainer => new();
	[Singleton] public JoypadPlayersMapping JoypadPlayersMapping => new();
	[Singleton] public GameLoader GameLoader => new();
	[Transient] public StageController StageControllerFactory => new(LayerConstants.LayerStageArea);
	[Transient] public StageCameraController StageCameraControllerFactory => new(LayerConstants.LayerStageArea);
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[Setting<bool>("Settings.Screen.PixelPerfect", SaveAs = "Video/PixelPerfect", Default = false)]
[Setting<bool>("Settings.Screen.Fullscreen", SaveAs = "Video/Fullscreen", Default = true)]
[Setting<bool>("Settings.Screen.VSync", SaveAs = "Video/VSync", Default = true)]
[Setting<bool>("Settings.Screen.Borderless", SaveAs = "Video/Borderless", Default = false)]
[Setting<Resolution>("Settings.Screen.WindowedResolution", SaveAs = "Video/WindowedResolution", DefaultAsString = "1920x1080")]
public class Settings {
	[Singleton] public ScreenSettingsManager ScreenSettingsManager => new(ApplicationConfig.Configuration);
	[Singleton] public SettingsContainer SettingsContainer => new(new ConfigFileWrapper(AppTools.GetUserFile("settings.ini")));
}

[Configuration]
[Loader("GameLoader", Tag = "main")]
[Preload<Texture2D>("Icon", "res://icon.png")]
[Resource<Theme>("MyTheme", "res://Platform/Assets/UI/my_theme2.tres")]
[Resource<Theme>("DebugConsoleTheme", "res://Platform/Assets/UI/DebugConsole.tres")]
[Resource<Texture2D>("Xbox360Buttons", "res://Platform/Assets/UI/Consoles/Xbox 360 Controller Updated.png")]
[Resource<Texture2D>("XboxOneButtons", "res://Platform/Assets/UI/Consoles/Xbox One Controller Updated.png")]
[Scene.Transient<RedefineActionButton>("RedefineActionButton", "res://Platform/Scenes/UI/RedefineActionButton.tscn")]
[Scene.Transient<ModalBoxConfirm>("ModalBoxConfirm", "res://Platform/Scenes/Menu/ModalBoxConfirm.tscn")]
[Scene.Transient<PlayerHud>("PlayerHudFactory", "res://Platform/Scenes/UI/PlayerHud.tscn")]
[Scene.Transient<Game>("GameSceneFactory", "res://Src/Managers/Game.tscn")]
[Scene.Singleton<MainMenu>("MainMenuSceneFactory", "res://Platform/Scenes/Menu/MainMenu.tscn")]
[Scene.Singleton<BottomBar>("BottomBarSceneFactory", "res://Platform/Scenes/Menu/BottomBar.tscn")]
[Scene.Singleton<PauseMenu>("PauseMenuSceneFactory", "res://Platform/Scenes/Menu/PauseMenu.tscn")]
[Scene.Singleton<SettingsMenu>("SettingsMenuSceneFactory", "res://Platform/Scenes/Menu/SettingsMenu.tscn")]
[Scene.Singleton<ProgressScreen>("ProgressScreenFactory", "res://Platform/Scenes/Menu/ProgressScreen.tscn")]
public class MainResources {
}

[Configuration]
[Loader("GameLoader", Tag = "game")]
[Resource<Texture2D>("Pickups", "res://Platform/Assets/pickups.png")]
[Resource<Texture2D>("Pickups2", "res://Platform/Assets/pickups2.png")]
[Resource<Texture2D>("LeonKnifeAnimationSprite", "res://Platform/Assets/Characters/Player-Leon/Leon-knife.png")]
[Resource<Texture2D>("LeonMetalbarAnimationSprite", "res://Platform/Assets/Characters/Player-Leon/Leon-metalbar.png")]
[Resource<Texture2D>("LeonGun1AnimationSprite", "res://Platform/Assets/Characters/Player-Leon/Leon-gun1.png")]
[Scene.Transient<InventorySlot>("InventorySlotResource", "res://Platform/Scenes/UI/InventorySlot.tscn")]
[Scene.Transient<WorldScene>("World3", "res://Worlds/World3.tscn")]
[Scene.Transient<PlayerNode>("PlayerNode", "res://Platform/Scenes/Player.tscn")]
[Scene.Transient<ZombieNode>("ZombieNode", "res://Platform/Scenes/Zombie2.tscn")]
[Scene.Transient<ProjectileTrail>("ProjectileTrail", "res://Platform/Scenes/ProjectileTrail.tscn")]
[Scene.Transient<PickableItemNode>("PickableItem", "res://Platform/Scenes/PickableItem.tscn")]
public class GameResources {
}

[Configuration]
[PoolContainer<Node>("PoolNodeContainer")]
public class PoolConfig {
	[Pool] NodePool<PlayerNode> PlayerPool => new("PlayerNode");
	[Pool] NodePool<ZombieNode> ZombiePool => new("ZombieNode");
	[Pool] NodePool<ProjectileTrail> ProjectilePool => new("ProjectileTrail");
	[Pool] NodePool<PickableItemNode> PickableItemPool => new("PickableItem");
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("PlayerActionsContainer")]
public class Actions {

	[AxisAction(SaveAs = "Controls/Lateral")] 
	private AxisAction Lateral => AxisAction.Create().Build();

	[AxisAction] 
	private AxisAction Vertical => AxisAction.Create().Build();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Up")]
	private InputAction Up => InputAction.Create()
		.Keys(Key.Up)
		.Buttons(JoyButton.DpadUp)
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Down")]
	private InputAction Down => InputAction.Create()
		.Keys(Key.Down)
		.Buttons(JoyButton.DpadDown)
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Left")]
	private InputAction Left => InputAction.Create()
		.Keys(Key.Left)
		.Buttons(JoyButton.DpadLeft)
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Right")]
	private InputAction Right => InputAction.Create()
		.Keys(Key.Right)
		.Buttons(JoyButton.DpadRight)
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();


	[InputAction(SaveAs = "Controls/Jump")]
	private InputAction Jump => InputAction.Create()
		.Keys(Key.Space)
		.Buttons(JoyButton.A)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Attack")]
	private InputAction Attack => InputAction.Create()
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction()]
	private InputAction Drop => InputAction.Create()
		.Keys(Key.G)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/NextItem")]
	private InputAction NextItem => InputAction.Create()
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/PrevItem")]
	private InputAction PrevItem => InputAction.Create()
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Pausable()
		.AsExtendedUnhandled();

	[InputAction(SaveAs = "Controls/Float")]
	private InputAction Float => InputAction.Create()
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Pausable()
		.AsExtendedUnhandled();
}

[Configuration]
[InputActionsContainer("UiActionsContainer")]
public class UiActions {
	[AxisAction] 
	private AxisAction UiVertical => AxisAction.Create().Build();

	[AxisAction] 
	private AxisAction UiLateral => AxisAction.Create().Build();

	[InputAction(AxisName = "UiVertical")]
	private InputAction UiUp => InputAction.Create("ui_up")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiVertical")]
	private InputAction UiDown => InputAction.Create("ui_down")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiLateral")]
	private InputAction UiLeft => InputAction.Create("ui_left")
		.KeepProjectSettings()
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction(AxisName = "UiLateral")]
	private InputAction UiRight => InputAction.Create("ui_right")
		.KeepProjectSettings()
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.AsGodotInput();

	[InputAction]
	private InputAction UiAccept => InputAction.Create("ui_accept")
		.KeepProjectSettings()
		.Buttons(JoyButton.A)
		.AsGodotInput();

	[InputAction]
	private InputAction UiSelect => InputAction.Create("ui_select")
		.KeepProjectSettings()
		.AsGodotInput();

	[InputAction]
	private InputAction UiCancel => InputAction.Create("ui_cancel")
		.KeepProjectSettings()
		.Buttons(JoyButton.B)
		.AsGodotInput();

	[InputAction]
	private InputAction ControllerSelect => InputAction.Create("select")
		.Keys(Key.Tab)
		.Buttons(JoyButton.Back)
		.AsGodotInput();

	[InputAction]
	private InputAction ControllerStart => InputAction.Create("start")
		.Keys(Key.Escape)
		.Buttons(JoyButton.Start)
		.AsGodotInput();
}

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("UiActionsContainer")]
public class OtherActions {
	[InputAction]
	private InputAction DebugOverlayAction => InputAction.Create("DebugOverlay")
		.Keys(Key.F9)
		.AsGodotInput();

}