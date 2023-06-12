using System;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Veronenger.Game.Character.InputActions;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game.Character.Player; 

public partial class PlayerNode {

	private readonly PlayerJoypadController _joypadController = new();
	private float XInput => _joypadController.Lateral.Strength;
	private float YInput => _joypadController.Vertical.Strength;
	private bool IsPressingRight => _joypadController.Right.IsPressed;
	private bool IsPressingLeft => _joypadController.Left.IsPressed;
	private bool IsPressingUp => _joypadController.Up.IsPressed;
	private bool IsPressingDown => _joypadController.Down.IsPressed;
	private InputAction Jump => _joypadController.Jump;
	private InputAction Attack => _joypadController.Attack;
	private InputAction Float => _joypadController.Float;
	private InputAction NextItem => _joypadController.NextItem;
	private InputAction PrevItem => _joypadController.PrevItem;
	private InputAction Drop => _joypadController.Drop;

	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	public PlayerMapping PlayerMapping { get; set; }

	[Inject] private ILazy<SettingsMenu> SettingsMenuSceneFactory { get; set; }

	public void SetPlayerMapping(PlayerMapping playerMapping) {
		Name = $"Player{playerMapping.Player}";
		Label.Text = $"P{playerMapping.Player}";
		PlayerMapping = playerMapping;
		_joypadController.Configure(PlayerMapping, PlayerActionsContainer, (inputAction, updater) => {
			// Player 1 uses all the mapping (keys included), player 2 and so on uses only the joypad
			UpdateInputAction(updater, inputAction);
		});
		PlayerMapping.OnJoypadChanged += () => {
			//Console.WriteLine("OnJoypadChanged:"+PlayerMapping);
		};
		PlayerMapping.OnJoypadConnect += () => {
			//Console.WriteLine("OnJoypadConnect:"+PlayerMapping);
		};
		PlayerMapping.OnJoypadDisconnect += () => {
			//Console.WriteLine("OnJoypadDisconnect:"+PlayerMapping);
		};
	}

	private void ConfigureInputActions() {
		// Update action on redefine
		SettingsMenuSceneFactory.Get().OnRedefine += OnRedefineAction;
		TreeExiting += () => {
			SettingsMenuSceneFactory.Get().OnRedefine -= OnRedefineAction;
			_joypadController.Disconnect();
		};
	}

	private void OnRedefineAction(InputAction from) {
		var name = $"{from.Name}/{PlayerMapping.Player}";
		var found = _joypadController.InputActionsContainer!.InputActionList.Find(i => i.Name == name);
		if (found is InputAction inputAction) {
			inputAction.Update(updater => UpdateInputAction(updater, from));
		} else {
			throw new Exception($"Action not found: {from.Name}");
		}
	}

	private void UpdateInputAction(InputAction.Updater updater, InputAction from) {
		if (PlayerMapping.Player == 0) updater.CopyAll(from);
		else updater.CopyJoypad(from);
		updater.SetJoypadId(PlayerMapping.JoypadId);
	}
}
