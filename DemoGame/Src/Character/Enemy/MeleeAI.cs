using System;
using Betauer.Core.Time;
using Betauer.StateMachine.Sync;
using Godot;
using Veronenger.Character.Handler;

namespace Veronenger.Character.Enemy;

public class ZombieAI : StateMachineSync<ZombieAI.State, ZombieAI.Event>, ICharacterAI {
    private readonly CharacterController _controller;
    private readonly Sensor _sensor;
    private readonly GodotStopwatch _stateTimer = new GodotStopwatch().Start();
    private readonly GodotStopwatch _inStateTimer = new GodotStopwatch().Start();

    public enum State { 
        Patrol,
        PatrolStop,
        Confusion,
        Attacked,
        EndAttack,
        ChasePlayer,
        Flee
    }
    
    public enum Event {
    }

    public static ICharacterAI Create(ICharacterHandler handler, Sensor sensor) {
        if (handler is CharacterController controller) return new ZombieAI(controller, sensor);
        if (handler is InputActionCharacterHandler) return DoNothingAI.Instance;
        throw new Exception($"Unknown handler: {handler.GetType()}");
    }

    public ZombieAI(CharacterController controller, Sensor sensor) : base(State.Patrol, "ZombieIA") {
        _controller = controller;
        _sensor = sensor;
        Config();
    }

    public string GetState() {
        return CurrentState.Key.ToString();
    }

    private void Config() {
        State(State.Patrol)
            .Enter(() => _stateTimer.Reset())
            .Execute(() => Advance(0.5f))
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(_sensor.IsOnWall).Set(State.PatrolStop)
            .If(_sensor.IsFloorFinishing).Set(State.PatrolStop)
            .If(() => _stateTimer.Elapsed > 4f).Set(State.PatrolStop)
            .Build();
        
        State(State.Attacked).If(() => !_sensor.IsAttacked()).Set(State.EndAttack).Build();
        State(State.EndAttack)
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(() => _sensor.Status.HealthPercent < 0.25f).Set(State.Flee)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => true).Set(State.Confusion)
            .Build();
        
        State(State.Flee)
            .Enter(() => {
                _stateTimer.Reset();
            })
            .Execute(() => {
                _sensor.FaceOppositePlayer();
                Advance(2f);
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(() => _stateTimer.Elapsed > 6f).Set(State.PatrolStop)
            .Build();
        
        State(State.ChasePlayer)
            .Execute(() => {
                _sensor.FaceToPlayer();
                Advance();
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(() => !_sensor.IsPlayerInsight()).Set(State.Confusion)
            .Build();

        State(State.Confusion)
            .Enter(() => {
                _stateTimer.Reset();
                _inStateTimer.Reset();
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => _stateTimer.Elapsed > 1.2f * 6).Then((ctx) => {
                _sensor.Flip();
                return ctx.Set(State.Patrol);
            })
            .If(() => _inStateTimer.Elapsed > 1.2f).Then((ctx) => {
                _sensor.Flip();
                _inStateTimer.Reset();
                return ctx.None();
            })
            .Build();
        
        State(State.PatrolStop)
            .Enter(() => {
                _stateTimer.Reset();
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => _stateTimer.Elapsed > 4f).Then((ctx) => {
                _sensor.Flip();
                return ctx.Set(State.Patrol);
            })
            .Build();
    }

    private void Advance(float factor = 1f) {
        _controller.DirectionalController.XInput = (_sensor.IsFacingRight ? 1 : -1) * factor;
    }

    public void EndFrame() {
        // GD.Print("Pressed:"+_controller.Jump.IsPressed()+
        // " JustPressed:"+_controller.Jump.IsJustPressed()+
        // " Released:"+_controller.Jump.IsReleased());

        _controller.DirectionalController.XInput = 0;
        _controller.EndFrame();
    }

    public class Sensor {
        private readonly ZombieNode _zombieNode;
        private readonly KinematicPlatformMotion _body;
        private readonly Func<Vector2> GetPlayerGlobalPosition;

        public Sensor(ZombieNode zombieNode, KinematicPlatformMotion body, Func<Vector2> playerGlobalPosition) {
            _zombieNode = zombieNode;
            _body = body;
            GetPlayerGlobalPosition = playerGlobalPosition;
        }

        public bool IsFacingRight => _body.IsFacingRight;
        public void Flip() => _body.Flip();
        public bool IsAttacked() => _zombieNode.IsState(ZombieState.Attacked);
        public bool IsOnWall() => _body.IsOnWall();
        public bool IsPlayerInsight() => _zombieNode.FacePlayerDetector.IsColliding(); // || _zombieNode.BackPlayerDetector.IsColliding();

        public bool IsFloorFinishing() => IsFacingRight
            ? !_zombieNode.FinishFloorRight.IsColliding()
            : !_zombieNode.FinishFloorLeft.IsColliding();

        public void FaceOppositePlayer() => _body.FaceOppositeTo(GetPlayerGlobalPosition());
        public void FaceToPlayer() => _body.FaceTo(GetPlayerGlobalPosition());

        public EnemyStatus Status => _zombieNode.Status;
    }
}