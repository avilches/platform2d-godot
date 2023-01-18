using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Camera;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.StateMachine.Sync;
using Godot;
using Veronenger.Character.Enemy;
using Veronenger.Character.Handler;
using Veronenger.Character.Items;
using Veronenger.Managers;

namespace Veronenger.Character.Player; 

public partial class PlayerNode {

	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	public void ConfigureOverlay() {

		var overlay = DebugOverlayManager.Overlay(CharacterBody2D)
			.Title("Player")
			.SetMaxSize(1000, 1000);

		AddOverlayHelpers(overlay);
		AddOverlayStates(overlay);
		AddOverlayMotion(overlay);
		AddOverlayCollisions(overlay);

		// DebugOverlayManager.Overlay(this)
		//     .Title("Player")
		//     .Text("AnimationStack",() => _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Text("TweenStack", () => _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("DangerTween.PlayLoop", () => DangerTween.PlayLoop()).End()
		//         .Button("DangerTween.Stop", () => DangerTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("PulsateTween.PlayOnce", () => PulsateTween.PlayOnce()).End()
		//         .Button("PulsateTween.Stop", () => PulsateTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("SqueezeTween.PlayOnce(kill)", () => SqueezeTween.PlayOnce(true)).End()
		//         .Button("SqueezeTween.Stop", () => SqueezeTween.Stop()).End()
		//         .TypedNode);
	}

	public void AddOverlayHelpers(DebugOverlay overlay) {
		_jumpHelperMonitor = overlay.Text("JumpHelper");
		overlay.Text("CoyoteFallingTimer", () => _coyoteFallingTimer.ToString());
		_coyoteMonitor = overlay.Text("Coyote");
	}

	public void AddOverlayStates(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Text("State", () => CurrentState.Key.ToString()).EndMonitor()
			.CloseBox();
	}

	public void AddOverlayMotion(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
				.Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
					.AddSerie("MotionY").Load(() => PlatformBody.MotionY).EndSerie()
				.EndMonitor()
			.CloseBox()
			.GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor();

	}
	
	public void AddOverlayCollisions(DebugOverlay overlay) {    
		overlay
			.Graph("Floor", () => PlatformBody.IsOnFloor()).Keep(10).SetChartHeight(10)
				.AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
			.EndMonitor()
			.Text("Floor", () => PlatformBody.GetFloorCollisionInfo()).EndMonitor()
			.Text("Ceiling", () => PlatformBody.GetCeilingCollisionInfo()).EndMonitor()
			.Text("Wall", () => PlatformBody.GetWallCollisionInfo()).EndMonitor();
	}


}