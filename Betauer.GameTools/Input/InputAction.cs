using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.Nodes;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public enum InputActionBehaviour {
    /// <summary>
    /// It works only through Simulate*() methods
    /// - WasPressed() and WasReleased() don't work
    /// - Can't be saved
    /// - ProcessMode is ignored
    /// </summary>
    Fake,

    /// <summary>
    /// WasPressed() and WasReleased() works
    /// 
    /// If the action matches with a GUI input, both will be processed.
    /// Example: a click to attack will be triggered too if the click has been used to click a button.
    /// Example: an attack with the keyboard letter C will be processed if the C is typed inside a TextEdit
    /// Behind the scenes, the action is processed in _Input() 
    /// </summary>
    ProcessWithGui,
    
    /// <summary>
    /// WasPressed() and WasReleased() works
    /// If the action matches with a GUI input, the GUI input is processed and the action will be ignored.
    /// Example: a click to attack will not be triggered if the click has been used to click a button.
    /// Example: an attack with the keyboard letter C will be ignored if the C is typed inside a TextEdit 
    /// Behind the scenes, the action is processed in _UnhandledInput() 
    /// </summary>
    AllowGuiStop
}

public partial class InputAction : IAction {
    public static NormalBuilder Create(string name) => new(name);
    public static NormalBuilder Create(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

    public static ConfigurableBuilder Configurable(string name) => new(name);
    public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

    private readonly InputActionStateHandler _handler;
    
    public string Name { get; }

    public bool IsPressed => _handler.Pressed;
    public bool IsJustPressed => _handler.JustPressed;
    public bool IsJustReleased => _handler.JustReleased;
    public bool IsMetaOrCtrlPressed => _handler.CommandOrCtrlPressed;
    public bool IsCtrlPressed => _handler.CtrlPressed;
    public bool IsShiftPressed => _handler.ShiftPressed;
    public bool IsAltPressed => _handler.AltPressed;
    public bool IsMetaPressed => _handler.MetaPressed;
    public float PressedTime => _behaviour == InputActionBehaviour.Fake ? InputActionStateHandler.MaxPressedTime : _handler.PressedTime;
    public float ReleasedTime => _behaviour == InputActionBehaviour.Fake ? InputActionStateHandler.MaxPressedTime : _handler.ReleasedTime;
    public bool WasPressed(float elapsed) => PressedTime <= elapsed;
    public bool WasReleased(float elapsed) => ReleasedTime <= elapsed;

    public float Strength => _handler.Strength;
    public float RawStrength => _handler.RawStrength;

    public void SimulatePress(float strength = 1f) => _handler.SimulatePress(strength);
    public void SimulateRelease() => _handler.SimulatePress(0f);
    public void SimulateCtrl(bool pressed) => _handler.SimulateCtrl(pressed);
    public void SimulateShift(bool pressed) => _handler.SimulateShift(pressed);
    public void SimulateAlt(bool pressed) => _handler.SimulateAlt(pressed);
    public void SimulateMeta(bool pressed) => _handler.SimulateMeta(pressed);
    public void SimulateCommandOrCtrl(bool pressed) => _handler.SimulateCommandOrCtrl(pressed);
    
    public bool IsEvent(InputEvent e) => Matches(e);
    public bool IsEventPressed(InputEvent e) => Matches(e) && e.IsPressed();
    public bool IsEventJustPressed(InputEvent e) => Matches(e) && e.IsJustPressed();
    public bool IsEventReleased(InputEvent e) => Matches(e) && e.IsReleased();

    public List<JoyButton> Buttons { get; } = new();
    public List<Key> Keys { get; } = new();
    public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
    public int AxisSign { get; private set; } = 1;
    public float DeadZone { get; private set; } = 0.5f;
    public MouseButton MouseButton { get; private set; } = MouseButton.None;
    public bool CommandOrCtrl { get; private set; }
    public bool Ctrl { get; private set; }
    public bool Shift { get; private set; }
    public bool Alt { get; private set; }
    public bool Meta { get; private set; }
    public Node.ProcessModeEnum ProcessMode => _handler.ProcessMode;

    public SaveSetting<string>? SaveSetting { get; private set; }
    
    [Inject] private Container Container { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    private readonly string? _inputActionsContainerName;
    private readonly string? _settingsContainerName;
    private readonly string? _settingsSection;
    private readonly bool _isConfigurable = false ;
    private readonly Updater _updater;
    private readonly InputActionBehaviour _behaviour;
    private readonly bool _configureGodotInputMap = false;

    public static InputAction Fake() => new InputAction(null,
        null,
        false,
        false,
        null,
        null,
        Node.ProcessModeEnum.Always, // Ignored in fake inputs because handler exists, but it's not added to NodeHandler
        InputActionBehaviour.Fake,
        false);

    private InputAction(string inputActionsContainerName, 
        string name, 
        bool keepProjectSettings,
        bool isConfigurable, 
        string? settingsContainerName, 
        string? settingsSection,
        Node.ProcessModeEnum processMode,
        InputActionBehaviour behaviour, 
        bool configureGodotInputMap) {
        Name = name;
        _handler = new InputActionStateHandler(this, processMode);
        _updater = new Updater(this); 
        _behaviour = behaviour;
        if (_behaviour != InputActionBehaviour.Fake) {
            _configureGodotInputMap = configureGodotInputMap;
            _inputActionsContainerName = inputActionsContainerName;
            _isConfigurable = isConfigurable;
            _settingsContainerName = settingsContainerName;
            _settingsSection = settingsSection;

            if (keepProjectSettings) LoadFromGodotProjectSettings();

            if (_behaviour == InputActionBehaviour.ProcessWithGui) {
                DefaultNodeHandler.Instance.OnInput(_handler);
                DefaultNodeHandler.Instance.OnProcess(_handler);
            } else if (_behaviour == InputActionBehaviour.AllowGuiStop) {
                DefaultNodeHandler.Instance.OnUnhandledInput(_handler);
                DefaultNodeHandler.Instance.OnProcess(_handler);
            }
        }
    }

    [PostInject]
    private void Configure() {
        if (_behaviour == InputActionBehaviour.Fake) return;
        
        // Configure and load settings
        if (_isConfigurable) {
            var section = _settingsSection ?? "Controls";
            var setting = Setting<string>.Save(_settingsContainerName, section, Name, Export());
            Container.InjectServices(setting);
            setting.ConfigureAndAddToSettingContainer();
            SetSaveSettings(setting);
            
            // Load settings from file
            Load();
        }
        
        // Add to InputContainer
        var inputActionsContainer = _inputActionsContainerName != null
            ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
            : Container.Resolve<InputActionsContainer>();
        inputActionsContainer.Add(this);
        
        SetupGodotInputMap();
    }

    public void Enable(bool enabled = true) {
        if (_behaviour == InputActionBehaviour.Fake) return;
        if (enabled) {
            _handler.Enable();
            SetupGodotInputMap();
        } else {
            Disable();
        }
    }

    public void Disable() {
        if (_behaviour == InputActionBehaviour.Fake) return;
        _handler.ClearState();
        _handler.Disable();
        if (_configureGodotInputMap && InputMap.HasAction(Name)) InputMap.EraseAction(Name);
    }

    public void SetupGodotInputMap() {
        if (_behaviour == InputActionBehaviour.Fake || !_configureGodotInputMap) return;
        
        if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);
        InputMap.AddAction(Name, DeadZone);

        CreateInputEvents().ForEach(e => InputMap.ActionAddEvent(Name, e));
    }

    private List<InputEvent> CreateInputEvents() {
        void AddModifiers(InputEventWithModifiers e) {
            e.ShiftPressed = Shift;
            e.AltPressed = Alt;
            if (CommandOrCtrl) {
                e.CommandOrControlAutoremap = true;
            } else {
                e.CtrlPressed = Ctrl;
                e.MetaPressed = Meta;
            }
        }
        
        List<InputEvent> events = new List<InputEvent>(Keys.Count + Buttons.Count + 1);
        foreach (var key in Keys) {
            var e = new InputEventKey();
            e.Keycode = key;
            AddModifiers(e);
            events.Add(e);
        }
        if (MouseButton != MouseButton.None) {
            var e = new InputEventMouseButton();
            e.ButtonIndex = MouseButton;
            AddModifiers(e);
            events.Add(e);
        }
        foreach (var button in Buttons) {
            var e = new InputEventJoypadButton();
            // e.Device = -1; // TODO: you can add a device id here
            e.ButtonIndex = button;
            events.Add(e);
        }

        if (Axis != JoyAxis.Invalid && AxisSign != 0) {
            var e = new InputEventJoypadMotion();
            e.Device = -1; // TODO: you can add a device id here
            e.Axis = Axis;
            e.AxisValue = AxisSign;
            events.Add(e);
        }
        return events;
    }

    public void LoadFromGodotProjectSettings() {
        if (_behaviour == InputActionBehaviour.Fake) return;

        if (!InputMap.HasAction(Name)) {
            GD.PushWarning($"{nameof(LoadFromGodotProjectSettings)}: Action {Name} not found in project");
            return;
        }
        
        foreach (var inputEvent in InputMap.ActionGetEvents(Name)) {
            if (inputEvent is InputEventKey key) {
                Keys.Add(key.Keycode);
            } else if (inputEvent is InputEventJoypadButton button) {
                Buttons.Add(button.ButtonIndex);
            } else if (inputEvent is InputEventJoypadMotion motion) {
                // TODO: feature missing, not tested!!!
                Axis = motion.Axis;
                AxisSign = (int)motion.AxisValue;
            } else if (inputEvent is InputEventMouseButton mouseButton) {
                MouseButton = mouseButton.ButtonIndex;
            }
        }
    }

    public InputAction SetSaveSettings(SaveSetting<string> saveSetting) {
        SaveSetting = saveSetting;
        return this;
    }

    public bool Matches(InputEvent e) =>
        e switch {
            InputEventKey key => Keys.Contains(key.Keycode),
            InputEventMouseButton mouse => MouseButton == mouse.ButtonIndex,
            InputEventJoypadButton button => Buttons.Contains(button.ButtonIndex),
            InputEventJoypadMotion motion => motion.Axis == Axis,
            _ => false
        };

    public InputAction SetProcessMode(Node.ProcessModeEnum processMode) {
        _handler.ProcessMode = processMode;
        return this;
    }

    public bool HasMouseButton() {
        return MouseButton != MouseButton.None;
    }

    public bool HasAxis() {
        return Axis != JoyAxis.Invalid;
    }

    public bool HasKey(Key key) {
        if (Keys.Count == 0) return false;
        if (Keys.Count == 1) return Keys[0] == key;
        for (var i = 2; i < Keys.Count; i++) if (Keys[i] == key) return true;
        return false;
    }

    public bool HasButton(JoyButton button) {
        if (Buttons.Count == 0) return false;
        if (Buttons.Count == 1) return Buttons[0] == button;
        for (var i = 2; i < Buttons.Count; i++) if (Buttons[i] == button) return true;
        return false;
    }

    public InputAction ResetToDefaults() {
        if (_behaviour == InputActionBehaviour.Fake) return this;
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Import(SaveSetting.DefaultValue);
        return this;
    }
    
    public InputAction Load() {
        if (_behaviour == InputActionBehaviour.Fake) return this;
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Import(SaveSetting.Value);
        return this;
    }
    
    public InputAction Save() {
        if (_behaviour == InputActionBehaviour.Fake) return this;
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Value = Export();
        return this;
    }

    public InputAction Update(Action<Updater> updater, bool setupGodotInputMap = true, bool save = true) {
        var (backupButtons, backupKeys, backupMouse) = (Buttons.ToArray(), Keys.ToArray(), MouseButton);
        var (axis, axisSign, backupDeadZone) = (Axis, AxisSign, DeadZone);
        var (commandOrCtrl, ctrl, shift, alt, meta) = (CommandOrCtrl, Ctrl, Shift, Alt, Meta);
        try {
            updater.Invoke(_updater);
            if (setupGodotInputMap) SetupGodotInputMap();
            if (save && SaveSetting != null) Save();
        } catch (Exception e) {
            _updater.SetButtons(backupButtons)
                .SetKeys(backupKeys)
                .SetMouse(backupMouse)
                .SetAxis(axis)
                .SetAxisSign(axisSign)
                .SetDeadZone(backupDeadZone)
                .WithCommandOrCtrl(commandOrCtrl)
                .WithCtrl(ctrl)
                .WithShift(shift)
                .WithAlt(alt)
                .WithMeta(meta);
        }
        return this;
    }

    public string Export() {
        var export = new List<string>(Keys.Count + Buttons.Count + 1);
        export.AddRange(Keys.Select(key => $"Key:{key}"));
        export.AddRange(Buttons.Select(button => $"Button:{button}"));
        if (Axis != JoyAxis.Invalid) {
            export.Add($"Axis:{Axis}");
        }
        return string.Join(",", export);
    }

    public InputAction Import(string export) {
        if (string.IsNullOrWhiteSpace(export)) return this;
        Update(updater => {
            updater.ClearButtons();
            updater.ClearKeys();
            updater.ClearAxis();
            export.Split(",").ToList().ForEach(ImportItem);
        });
        return this;
    }

    private void ImportItem(string item) {
        if (!item.Contains(':')) return;
        var parts = item.Split(":");
        var key = parts[0].ToLower().Trim();
        var value = parts[1].Trim();
        if (key == "key") {
            ImportKey(value);
        } else if (key == "button") {
            ImportButton(value);
        } else if (key == "axis") {
            ImportAxis(value);
        }
    }

    private bool ImportKey(string value) {
        try {
            var key = int.TryParse(value, out _) ? (Key)value.ToInt() : Parse<Key>(value); 
            Keys.Add(key);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportButton(string value) {
        try {
            var joyButton = int.TryParse(value, out _) ? (JoyButton)value.ToInt() : Parse<JoyButton>(value); 
            Buttons.Add(joyButton);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportAxis(string value) {
        try {
            var axis = int.TryParse(value, out _) ? (JoyAxis)value.ToInt() : Parse<JoyAxis>(value); 
            Axis = axis;
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);
}