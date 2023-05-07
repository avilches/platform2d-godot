using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Tools.FastReflection;
using TypeExtensions = Betauer.Core.TypeExtensions;

namespace Betauer.Input.Joypad;

public class JoypadController {
    public InputActionsContainer InputActionsContainer { get; private set; }
    private InputActionsContainer _source;
    private PlayerMapping _playerMapping;

    private List<ISetter>? _fastSetters;

    protected List<ISetter> FastSetters => _fastSetters ??=
        GetType().GetProperties()
            .Where(p => TypeExtensions.ImplementsInterface(p.PropertyType, typeof(IAction)))
            .Select(p => new FastSetter(p) as ISetter)
            .ToList();
    
    public void Configure(InputActionsContainer source, PlayerMapping playerMapping) {
        _playerMapping = playerMapping;
        _source = source;
        Reconnect();
        playerMapping.OnJoypadChanged += Reconnect;
    }
    
    private void Reconnect() {
        Disconnect();
        var suffix = $"{_playerMapping.Player}/{_playerMapping.JoypadId}";
        InputActionsContainer = _source.Clone(_playerMapping.JoypadId, suffix);
        InputActionsContainer.Enable();
        
        FastSetters.ForEach(setter => {
            var name = $"{setter.Name}/{suffix}";
            var action = InputActionsContainer.FindAction(name);
            if (action == null) throw new Exception($"Action {name} not found");
            setter.SetValue(this, action);
        });
    }

    public void Disconnect() {
        InputActionsContainer?.Disable();
        InputActionsContainer?.QueueFree();
        InputActionsContainer = null;
    }
}