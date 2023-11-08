using System.Linq;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Camera.Control;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.FSM.Sync;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.UI;
using Godot;

namespace Veronenger.Game.RTS.World;

public partial class RtsWorld : Node, IInjectable {
	[Inject] private WorldGenerator WorldGenerator { get; set; }
	[Inject] private GameObjectRepository GameObjectRepository { get; set; }
	[Inject] public RtsConfig RtsConfig { get; private set; }
	[Inject] public CameraContainer CameraContainer { get; private set; }
	[Inject] protected DebugOverlayManager DebugOverlayManager { get; set; }
	private readonly DragCameraController _dragCameraController = new();

	[NodePath("TerrainTileMap")] private TileMap TerrainTileMap { get; set; }
	[NodePath("TextureHeight")] private Sprite2D TextureHeight { get; set; }
	[NodePath("TextureHumidity")] private Sprite2D TextureHumidity { get; set; }
	[NodePath("TexturePoisson")] private Sprite2D TexturePoisson { get; set; }

	private CameraController CameraController;
	private CameraGameObject CameraGameObject;
	private Camera2D Camera => CameraController.Camera2D;

	public enum RtsState {
		DoNothing,
		Idle
	}

	public enum RtsTransition {
		Idle,
	}

	private readonly FsmNodeSync<RtsState, RtsTransition> _fsm = new(RtsState.DoNothing, "ScreenState.FSM", true);

	public void PostInject() {
		_fsm.On(RtsTransition.Idle).Set(RtsState.Idle);

		_fsm.State(RtsState.DoNothing)
			.Enter(() => { _dragCameraController.Enable(false); })
			.Build();

		_fsm.State(RtsState.Idle)
			.Enter(() => { _dragCameraController.Enable(); })
			.OnInput(Zooming)
			.Build();

		_fsm.Execute();

		AddChild(_fsm);
	}
	
	private void SetSeed(string seed) {
		if (seed.IsValidInt() && seed.ToInt() != WorldGenerator.BiomeGenerator.Seed)  {
			WorldGenerator.BiomeGenerator.Seed = seed.ToInt();
			WorldGenerator.Generate();
		}
	}

	private void Zooming(InputEvent @event) {
		if (@event.IsKeyJustPressed(Key.Q) || @event.IsClickPressed(MouseButton.WheelUp)) {
			if (CameraGameObject.ZoomLevel == RtsConfig.ZoomLevels.Count - 1) return;
			var targetZoom = RtsConfig.ZoomLevels[++CameraGameObject.ZoomLevel];
			CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

			GetViewport().SetInputAsHandled();
		} else if (@event.IsKeyJustPressed(Key.E) || @event.IsClickPressed(MouseButton.WheelDown)) {
			if (CameraGameObject.ZoomLevel == 0) return;
			var targetZoom = RtsConfig.ZoomLevels[--CameraGameObject.ZoomLevel];
			CameraController.Zoom(new Vector2(targetZoom, targetZoom), RtsConfig.ZoomTime, Easings.Linear, CameraController.Camera2D.GetLocalMousePosition);

			GetViewport().SetInputAsHandled();
		}
	}

	public void SetMainCamera(Camera2D camera2D) {
		CameraController = CameraContainer.Camera(camera2D);
		_dragCameraController.Attach(camera2D).WithMouseButton(MouseButton.Left).Enable(false);
	}

	public async void StartNewGame() {
		CameraGameObject = GameObjectRepository.Create<CameraGameObject>("ScreenState", "ScreenState");
		Init();
		TerrainTileMap.Clear();
		WorldGenerator.Configure(TerrainTileMap);
		WorldGenerator.Generate();
		ConfigureDebugOverlay();
		// var poissonDemos = new PoissonDemos(TextureTerrainMap, TexturePoisson);
		// AddChild(poissonDemos);
		// poissonDemos.QueueFree();
	}

	public void LoadGame(RtsSaveGameConsumer consumer) {
		CameraGameObject = GameObjectRepository.Get<CameraGameObject>("ScreenState");
		Init();
		// Load the values from the save game
		ConfigureDebugOverlay();
		CameraController.Camera2D.Position = CameraGameObject.Position;
	}

	private void Init() {
		CameraGameObject.Configure(Camera);
		var zoom = RtsConfig.ZoomLevels[CameraGameObject.ZoomLevel];
		CameraController.Camera2D.Zoom = new Vector2(zoom, zoom);
		_fsm.Send(RtsTransition.Idle);
	}
	
	private void ConfigureDebugOverlay() {
		var viewGroup = new ButtonGroup();

		DebugOverlayManager.Overlay("RTS")
			.OnDestroy(() => viewGroup.Dispose())
			.SetMinSize(400, 100)
			.Edit("Seed", "100", SetSeed).SetMinSize(20).EndMonitor()
			.Add(new HBoxContainer().NodeBuilder()
				.Label("View Mode").End()
				.ToggleButton("Terrain", (button) => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Terrain;
					WorldGenerator.UpdateView();
				}, () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Terrain, viewGroup).End()
				.ToggleButton("Height", (button) => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Height;
					WorldGenerator.UpdateView();
				}, () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Terrain, viewGroup).End()
				.ToggleButton("Humidity", (button) => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.Humidity;
					WorldGenerator.UpdateView();
				}, () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.Terrain, viewGroup).End()
				.ToggleButton("FallOff", (button) => {
					WorldGenerator.CurrentViewMode = WorldGenerator.ViewMode.FalloffMap;
					WorldGenerator.UpdateView();
				}, () => WorldGenerator.CurrentViewMode == WorldGenerator.ViewMode.FalloffMap, viewGroup).End()
				.End())
			.Text("Humidity").EndMonitor()
			.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HeightNoise.Frequency, (value) => {
				WorldGenerator.BiomeGenerator.HeightNoise.Frequency = value;
				WorldGenerator.Generate();
			}).EndMonitor()
			.Text("Humidity").EndMonitor()
			.Edit("Frequency", () => WorldGenerator.BiomeGenerator.HumidityNoise.Frequency, (value) => {
				WorldGenerator.BiomeGenerator.HumidityNoise.Frequency = value;
				WorldGenerator.Generate();
			}).EndMonitor()
			;

	}

}
