using Godot;using Tools.Input;using Tools.Statemachine;using Veronenger.Game.Controller.Character;namespace Veronenger.Game.Character.Player.States {    public abstract class PlayerState : State {        protected readonly PlayerController Player;        protected PlayerState(PlayerController player) {            Player = player;        }        public void Debug(bool flag, string message) {            if (flag) Debug(message);        }        public void Debug(string message) {            GD.Print($"#{Player.Frame}: {GetType().Name} | {message}");        }        // Input from the player        protected float XInput => Player.PlayerActions.LateralMotion.Strength;        protected float YInput => Player.PlayerActions.VerticalMotion.Strength;        protected ActionState Jump => Player.PlayerActions.Jump;        protected ActionState Attack => Player.PlayerActions.Attack;        protected bool IsRight => XInput > 0;        protected bool IsLeft => XInput < 0;        protected bool IsUp => YInput < 0;        protected bool IsDown => YInput > 0;        protected Vector2 Motion => Player.Motion;        public PlayerConfig PlayerConfig => Player.PlayerConfig;    }}