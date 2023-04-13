using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Lifecycle;
using Betauer.Application.Monitor;
using Betauer.Application.Notifications;
using Betauer.Application.Screen;
using Betauer.Camera;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.FSM;
using Godot;
using Veronenger.Character.Player;
using PropertyTweener = Betauer.Animation.PropertyTweener;
using ZombieNode = Veronenger.Character.Npc.ZombieNode;

namespace Veronenger.Managers.Autoload; 

public partial class Bootstrap : Node /* needed to be instantiated as an Autoload from Godot */ {
	private static readonly Logger Logger = LoggerFactory.GetLogger<Bootstrap>();

	[Inject] private NotificationsHandler NotificationsHandler { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private WindowNotificationStatus WindowNotificationStatus { get; set; }

	public Bootstrap() {
		AppTools.ConfigureExceptionHandlers(GetTree);

		new GodotContainer(this)
			.Start(options => {
				options
					.ScanConfiguration(new DefaultConfiguration(GetTree()))
					.Scan(GetType().Assembly);
			});

#if DEBUG
		DevelopmentConfig();
#else
			ExportConfig();
#endif
			
		GD.Print(string.Join("\n", Project.GetOSInfo()));
		GD.Print(string.Join("\n", Project.GetSettings(
			"debug/file_logging/enable_file_logging",
			"debug/file_logging/enable_file_logging.pc",
			"debug/file_logging/log_path",
			"debug/file_logging/log_path.standalone",
			"application/run/disable_stdout",
			"application/run/disable_stderr",
			"application/run/flush_stdout_on_print",
			"application/run/flush_stdout_on_print.debug",
			"application/config/use_custom_user_dir",
			"application/config/project_settings_override",
			"application/config/version"
		)));
	}

	public override void _Ready() {
		Name = nameof(Bootstrap); // This name is shown in the remote editor
		Logger.Info($"Bootstrap time: {Project.Uptime.TotalMilliseconds} ms");
		NotificationsHandler.OnWMCloseRequest += () => {
			GD.Print($"[WmQuitRequest] Uptime: {Project.Uptime.TotalMinutes:0} min {Project.Uptime.Seconds:00} sec");
		};
		DebugOverlayManager.DebugConsole.AddAllCommands(WindowNotificationStatus);
	}

	private static void ExportConfig() {
		LoggerFactory.SetDefaultTraceLevel(TraceLevel.Warning);

		// Bootstrap logs, always log everything :)
		LoggerFactory.SetTraceLevel<Bootstrap>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ResourceLoaderContainer>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ConfigFileWrapper>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<BaseScreenResolutionService>(TraceLevel.All);
	}

	private static void DevelopmentConfig() {
		// All enabled, then disabled one by one, so developers can enable just one 
		LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);

		// Bootstrap logs, always log everything :)
		LoggerFactory.SetTraceLevel<Bootstrap>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ConfigFileWrapper>(TraceLevel.All);

		// DI
		LoggerFactory.SetTraceLevel<Provider>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Betauer.DI.Container>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Betauer.DI.Container.Injector>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<Betauer.DI.Container.Scanner>(TraceLevel.Error);

		// GameTools
		LoggerFactory.SetTraceLevel<BaseScreenResolutionService>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<Fsm>(TraceLevel.Error);

		// Animation
		LoggerFactory.SetTraceLevel<CallbackNodeTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<CallbackTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<PauseTweener>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<PropertyTweener>(TraceLevel.Error);

		// Game
		LoggerFactory.SetTraceLevel<MainStateMachine>(TraceLevel.Error);
		LoggerFactory.SetTraceLevel<CameraStageLimiter>(TraceLevel.Error);

		// Player and enemies
		LoggerFactory.SetTraceLevel<PlayerNode>(TraceLevel.All);
		LoggerFactory.SetTraceLevel<ZombieNode>(TraceLevel.All);
			
		// LoggerFactory.OverrideTraceLevel(TraceLevel.All);
	}
}
