using System.Collections.Generic;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Camera;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Managers;

namespace Veronenger.Character.Player; 

public readonly struct Attack {
	public readonly float Damage;

	public Attack(float damage) {
		Damage = damage;
	}
}
	
public partial class PlayerNode : CharacterBody2D {
	[OnReady("Sprite2D")] private Sprite2D _mainSprite;
	[OnReady("AttackArea")] private Area2D _attackArea;
	[OnReady("DamageArea")] private Area2D _damageArea;
	[OnReady("RichTextLabel")] public RichTextLabel Label;
	[OnReady("Detector")] public Area2D PlayerDetector;
	[OnReady("Sprite2D/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[OnReady("Camera2D")] private Camera2D _camera2D;

	[OnReady("Marker2D")] public Marker2D Marker2D;
	[OnReady("FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private PlayerStateMachine StateMachine { get; set; } // Transient!
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private InputAction MMB { get; set; }

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationRun { get; private set; }
	public ILoopStatus AnimationJump { get; private set; }
	public ILoopStatus AnimationFall { get; private set; }
	public IOnceStatus AnimationAttack { get; private set; }
	public IOnceStatus AnimationJumpAttack { get; private set; }

	public IOnceStatus PulsateTween;
	public ILoopStatus DangerTween;
	public IOnceStatus SqueezeTween;

	private readonly DragCameraController _cameraController = new();
	private AnimationStack _animationStack;
	private AnimationStack _tweenStack;
	private Restorer _restorer;
	public IFlipper Flipper;

	public override void _Ready() {
			
		_animationStack = new AnimationStack("Player.AnimationStack").SetAnimationPlayer(_animationPlayer);
		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationRun = _animationStack.AddLoopAnimation("Run");
		AnimationJump = _animationStack.AddLoopAnimation("Jump");
		AnimationFall = _animationStack.AddLoopAnimation("Fall");
		AnimationAttack = _animationStack.AddOnceAnimation("Attack").OnStart(() => _attackArea.EnableAllShapes()).OnEnd(() => _attackArea.EnableAllShapes(false));
		AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

		_cameraController.WithAction(MMB).Attach(_camera2D);

		_tweenStack = new AnimationStack("Player.AnimationStack");
		_restorer = this.CreateRestorer(Properties.Modulate, Properties.Scale2D)
			.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
				
		_restorer.Save();
		PulsateTween = _tweenStack.AddOnceTween("Pulsate", CreateMoveLeft()).OnEnd(_restorer.Restore);
		DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger()).OnEnd(_restorer.Restore);
		SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze()).OnEnd(_restorer.Restore);

		Flipper = new FlipperList()
			.AddSprite(_mainSprite)
			.AddArea2D(_attackArea);
		_attackArea.EnableAllShapes(false);
		StateMachine.Start("Player", this);

		CharacterManager.RegisterPlayerNode(this);
		CharacterManager.PlayerConfigureCollisions(this);
		CharacterManager.PlayerConfigureAttackArea2D(_attackArea);

		PlayerDetector.CollisionLayer = 0;
		PlayerDetector.CollisionMask = 0;
			
		StageManager.ConfigureStageCamera(_camera2D, PlayerDetector);
			
		_attackArea.Monitoring = false;
		// CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

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

	private IAnimation CreateReset() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps(Properties.Modulate)
			.From(new Color(1, 1, 1, 0))
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
		// seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
		return seq;
	}

	private IAnimation CreateMoveLeft() {
		var seq = KeyframeAnimation.Create(_mainSprite)
			.SetDuration(2f)
			.AnimateKeys(Properties.Modulate)
			.KeyframeTo(0.25f, new Color(1, 1, 1, 0))
			.KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
			.KeyframeTo(1f, new Color(1, 1, 1, 1))
			.EndAnimate()
			.AnimateKeys<Vector2>(Properties.Scale2D)
			.KeyframeTo(0.5f, new Vector2(1.4f, 1f))
			.KeyframeTo(1f, new Vector2(1f, 1f))
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
		return seq;
	}

	private IAnimation CreateDanger() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps<Color>(Properties.Modulate, Easings.CubicInOut)
			.To(new Color(1, 0, 0, 1), 1)
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		return seq;
	}

	private IAnimation CreateSqueeze() {
		var seq = SequenceAnimation.Create(this)
			.AnimateSteps(Properties.Scale2D, Easings.SineInOut)
			.To(new Vector2(1.4f, 1f), 0.25f)
			.To(new Vector2(1f, 1f), 0.25f)
			.EndAnimate()
			.SetLoops(2);
		return seq;
	}

	public override void _Input(InputEvent e) {
		if (e.IsLeftDoubleClick()) _camera2D.Position = Vector2.Zero;
		if (e.IsKeyPressed(Key.Q)) {
			// _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
		} else if (e.IsKeyPressed(Key.W)) {
			// _camera2D.Zoom = new Vector2(1, 1);
		} else if (e.IsKeyPressed(Key.E)) {
			// _camera2D.Zoom += new Vector2(0.05f, 0.05f);
		}
	}

	public override void _Process(double delta) {
		QueueRedraw();
	}

	public override void _Draw() {
		foreach (var floorRaycast in FloorRaycasts) {
			DrawLine(floorRaycast.Position, floorRaycast.Position + floorRaycast.TargetPosition, Colors.Red, 1F);
		}
		// DrawLine(_floorRaycast.Position, GetLocalMousePosition(), Colors.Blue, 3F);
	}

	public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;

}