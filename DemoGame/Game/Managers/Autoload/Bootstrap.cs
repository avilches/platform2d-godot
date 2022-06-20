using System;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Bus;
using Betauer.DI;
using Betauer.Managers;
using Betauer.Memory;
using Betauer.Screen;
using Betauer.StateMachine;
using Veronenger.Game.Controller.Stage;
using Container = Betauer.DI.Container;
using TraceLevel = Betauer.TraceLevel;

namespace Veronenger.Game.Managers.Autoload {
    public class Bootstrap : GodotContainer /* needed to be instantiated as an Autoload from Godot */ {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Bootstrap));
        public static readonly DateTime StartTime = DateTime.Now;
        public static TimeSpan Uptime => DateTime.Now.Subtract(StartTime);

        public Bootstrap() {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            if (ApplicationConfig.IsExported()) {
                ExportConfiguration();
            } else {
                DevelopmentConfig();
            }
            ShowConfig();
            AutoConfigure();
            Logger.Info($"Container time: {Uptime.TotalMilliseconds} ms");
        }

        private static void ShowConfig() {
            Logger.Info("cmd line args: " + string.Join(" ", OS.GetCmdlineArgs()));
            Logger.Info("app version  : " +AppInfo.Version);
            Logger.Info("features     : " + string.Join(", ", FeatureFlags.GetActiveList()));
            Logger.Info("name host    : " + OS.GetName());
            Logger.Info("data dir     : " + OS.GetDataDir());
            Logger.Info("user data dir: " + OS.GetUserDataDir());
            Logger.Info("config dir   : " + OS.GetConfigDir());
            Logger.Info("cache dir    : " + OS.GetCacheDir());
            new[] {
                "logging/file_logging/enable_file_logging",
                "logging/file_logging/enable_file_logging.pc",
                "logging/file_logging/log_path",
                "logging/file_logging/log_path.standalone",
                "application/run/disable_stdout",
                "application/run/disable_stderr",
                "application/run/flush_stdout_on_print",
                "application/run/flush_stdout_on_print.debug",
                "application/config/use_custom_user_dir",
                "application/config/project_settings_override",
                "mono/unhandled_exception_policy",
                "mono/unhandled_exception_policy.standalone",
                "application/config/version"
            }.ToList()
                .ForEach(property => Logger.Info(property + ": " + ProjectSettings.GetSetting(property)));
        }

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            LoggerFactory.LoadFrames(GetTree().GetFrame);
        }

        private void ExportConfiguration() {
            DisposeTools.ShowShutdownWarning = false; 
            DisposeTools.ShowMessageOnCreate = false;
            LoggerFactory.SetConsoleOutput(ConsoleOutput.GodotPrint); // GD.Print means it appears in the user data logs
            LoggerFactory.IncludeTimestamp(true);
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(SettingsFile), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(GameManager), TraceLevel.All);
    }
        
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) {
            Logger.Error($"Got unhandled exception: {e.ExceptionObject}");
            // LoggerFactory.Dispose();
            // GetTree().Quit();
        }        

        private void DevelopmentConfig() {
            DisposeTools.ShowShutdownWarning = true;
            DisposeTools.ShowMessageOnCreate = false;
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine);
            LoggerFactory.IncludeTimestamp(true);

            // All enabled, then disabled one by one, so developers can enable just one 
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

            // Bootstrap logs, all always :)
            LoggerFactory.SetTraceLevel(typeof(Bootstrap), TraceLevel.All);

            // DI
            LoggerFactory.SetTraceLevel(typeof(ContainerBuilder), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(FactoryProvider<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(Injector), TraceLevel.Error);

            // GameTools
            LoggerFactory.SetTraceLevel(typeof(SettingsFile), TraceLevel.Debug);
            LoggerFactory.SetTraceLevel(typeof(GodotTopic<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(GodotListener<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(ObjectPool), TraceLevel.Error);

            // Animation
            LoggerFactory.SetTraceLevel(typeof(Launcher), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(MultipleSequencePlayer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(SingleSequencePlayer), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(RepeatablePlayer<>), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.Error);

            // Screen
            LoggerFactory.SetTraceLevel(typeof(FitToScreenResolutionService), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PixelPerfectScreenResolutionService), TraceLevel.Error);

            // Managers
            LoggerFactory.SetTraceLevel(typeof(GameManager), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StageManager), TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(StateMachine), "GameManager", TraceLevel.Error);

            // Player and enemies
            LoggerFactory.SetTraceLevel(typeof(StageCameraController), TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(StateMachine), "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Motion", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Collision", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("JumpHelper", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("CoyoteJump", "Player:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("JumpVelocity", "Player:*", TraceLevel.Error);

            LoggerFactory.SetTraceLevel(typeof(StateMachine), "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(AnimationStack), "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Motion", "Enemy.Zombie:*", TraceLevel.Error);
            LoggerFactory.SetTraceLevel("Collision", "Enemy.Zombie:*", TraceLevel.Error);
        }

        /**
         * Detect quit app (by ALT+F4, Command+Q or user menu)
         */
        public override void _Notification(int what) {
            if (what == MainLoop.NotificationWmQuitRequest) {
                Exit();
            }
        }

        private void Exit() {
            var timespan = Uptime;
            var elapsed = $"{(int)timespan.TotalMinutes} min {timespan.Seconds:00} sec";
            Logger.Info("User requested exit the application. Uptime: " + elapsed);
            LoggerFactory.Dispose(); // Please, do this the last so previous disposing operation can log
        }
    }
}