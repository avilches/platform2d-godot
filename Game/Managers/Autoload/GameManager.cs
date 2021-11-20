using System;
using System.Diagnostics;
using Godot;
using Tools;
using Tools.Bus;
using Veronenger.Game.Controller.Stage;
using Veronenger.Game.Tools.Resolution;
using Directory = System.IO.Directory;
using Path = System.IO.Path;
using TraceLevel = Tools.TraceLevel;

namespace Veronenger.Game.Managers.Autoload {
    /**
     * GameManager es Node para estar en autoload y recibir eventos
     * Crea automaticamente a los otros managers (Manager = siempre cargado)
     * Los Controller son scripts de objetos de le escena que se cargan y se registran en los managers
     *
     * Los Manager actuan de intermediarios entre objetos que no se conocen entre si. Por ejemplo: las death zones,
     * plataformas o stages se añaden en sus managers, que escucha a las señales que estos objetos producen.
     * Por otro lado, el jugador se registra en estos mismos manager para escuchar estas señales, sin llegar a saber
     * donde estan realmente estos objetos (plataformas o areas).
     *
     */
    public class GameManager : Node /* needed to receive _EnterTree and OnScreenResized signals */ {
        public static GameManager Instance { get; private set; }

        private readonly GodotDiRepository DiRepository = GodotDiRepository.Instance;

        private readonly AreaManager AreaManager;
        private readonly StageManager StageManager;
        private readonly PlatformManager PlatformManager;
        private readonly SlopeStairsManager SlopeStairsManager;
        private readonly CharacterManager CharacterManager;


        // public Logger Logger;
        private ScreenManager ScreenManager;

        public static Vector2 FULL_DIV6 = new Vector2(320, 180); // 1920x1080 / 6
        public static Vector2 FULL_DIV4 = new Vector2(480, 270); // 1920x1080 / 4
        public static Vector2 FULL_DIV2 = new Vector2(960, 540); // 1920x1080 / 2
        public static Vector2 FULLHD = new Vector2(1920, 1080);
        public static Vector2 FULLHDx133 = new Vector2(2560, 1440); // 1920x1080 * 1.33

        public GameManager() {
            if (Instance != null) {
                throw new Exception("Only one instance");
            }
            Instance = DiRepository.AddSingleton(this);
            SlopeStairsManager = DiRepository.AddSingleton(new SlopeStairsManager());
            AreaManager = DiRepository.AddSingleton(new AreaManager());
            StageManager = DiRepository.AddSingleton(new StageManager());
            PlatformManager = DiRepository.AddSingleton(new PlatformManager());
            CharacterManager = DiRepository.AddSingleton(new CharacterManager());
            DiRepository.Finish();
        }

        public void AutoWire(object instance) {
            DiRepository.InjectFields(instance);
        }

        public override void _EnterTree() {
            // MicroBenchmarks();
            var runningTests = GetTree().CurrentScene.Filename == "res://Tests/Runner/RunTests.tscn";
            if (runningTests) RunTests();
            else RunGame();
        }

        private void RunGame() {
            ScreenManager = new ScreenManager(FULL_DIV4, SceneTree.StretchMode.Viewport,
                SceneTree.StretchAspect.Keep);
            ScreenManager.Start(this, nameof(OnScreenResized));
            ScreenManager.SetAll(false, 3, false);

            ConfigureLoggerFactory();
        }

        private void RunTests() {
            ScreenManager = new ScreenManager(new Vector2(1200, 900), SceneTree.StretchMode.Disabled,
                SceneTree.StretchAspect.Expand);
            ScreenManager.Start(this, nameof(OnScreenResized));
            ScreenManager.SetAll(false, 1, false);
        }

        private bool _logToFileEnabled = false;

        private void ConfigureLoggerFactory() {
            if (_logToFileEnabled) {
                var folder = Directory.GetCurrentDirectory();
                var logPath = Path.Combine(folder, $"Veronenger.{DateTime.Now:yyyy-dd-M--HH-mm-ss}.log");
                LoggerFactory.AddFileWriter(logPath);
            }

            LoggerFactory.AddGodotPrinter();

            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Off);
            // Tools
            LoggerFactory.SetTraceLevel(typeof(GodotTopic<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(GodotListener<>), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Off);
            // Managers
            LoggerFactory.SetTraceLevel(typeof(ScreenManager), TraceLevel.Off);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Off);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "StateMachine", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Animation", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "JumpHelper", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "CoyoteJump", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "JumpVelocity", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Input", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Motion", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Player:*", "Collision", TraceLevel.Off);

            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "StateMachine", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", "Animation", TraceLevel.Off);
            LoggerFactory.SetTraceLevel("Enemy.Zombie:*", TraceLevel.Off);

            LoggerFactory.Start(this);
        }

        private void MicroBenchmarks() {
            var x = Stopwatch.StartNew();
            LoggerFactory.GetLogger("x");
            int calls = 100000;
            for (int i = 0; i < calls; i++) {
                LoggerFactory.GetLogger("x").Info("aaa "+i);
            }
            x.Stop();
            GD.Print($"{calls} calls in {x.Elapsed.ToString()}: {x.ElapsedMilliseconds/(float)calls} calls/ms. {(x.ElapsedMilliseconds*1000)/(float)calls} calls/s.");
            // Quit();
        }

        public sealed override void _PhysicsProcess(float delta) {
        }

        public void OnScreenResized() {
            ScreenManager.UpdateResolution();
        }

        public void ChangeScene(string scene) {
            // _logger.Debug($"Change scene to: {scene}");
            Error error = GetTree().ChangeScene(scene);
        }

        public void Quit() {
            LoggerFactory.Dispose();
            GetTree().Quit();
        }

    }
}