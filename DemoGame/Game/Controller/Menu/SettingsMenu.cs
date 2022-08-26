using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.Signal;
using Godot;
using Veronenger.Game.Controller.UI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class SettingsMenu : CanvasLayer {
        [OnReady("Panel")] 
        private Panel _panel;

        [OnReady("Panel/SettingsBox")] 
        private VBoxContainer _settingsBox;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Fullscreen")]
        private CheckButton _fullscreenButtonWrapper;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Resolution")]
        private Button _resolutionButton;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/PixelPerfect")]
        private CheckButton _pixelPerfectButtonWrapper;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/Borderless")]
        private CheckButton _borderlessButtonWrapper;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/VSync")]
        private CheckButton _vsyncButtonWrapper;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/GamepadControls")]
        private VBoxContainer _gamepadControls;

        [OnReady("Panel/SettingsBox/ScrollContainer/MarginContainer/Menu/KeyboardControls")]
        private VBoxContainer _keyboardControls;

        [OnReady("Panel/SettingsBox/ScrollContainer")]
        private ScrollContainer _scrollContainer;

        [OnReady("Panel/RedefineBox")] 
        private VBoxContainer _redefineBox;

        [OnReady("Panel/RedefineBox/Message")] 
        private Label _redefineActionMessage;

        [OnReady("Panel/RedefineBox/ActionName")] 
        private Label _redefineActionName;

        [Inject] private GameManager _gameManager { get; set; }
        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }
        [Inject] private InputAction UiLeft { get; set; }
        [Inject] private InputAction UiRight { get; set; }

        [Inject] private InputAction Attack { get; set; }
        [Inject] private InputAction Jump { get; set; }
        
        [Inject] private InputAction Up { get; set; }
        [Inject] private InputAction Down { get; set; }
        [Inject] private InputAction Left { get; set; }
        [Inject] private InputAction Right { get; set; }
        
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);

            ConfigureScreenSettingsButtons();
            ConfigureControls();

            _fullscreenButtonWrapper.Pressed = _screenSettingsManager.Fullscreen;
            _pixelPerfectButtonWrapper.Pressed = _screenSettingsManager.PixelPerfect;
            _vsyncButtonWrapper.Pressed = _screenSettingsManager.VSync;
            _borderlessButtonWrapper.Pressed = _screenSettingsManager.Borderless;
            _borderlessButtonWrapper.SetFocusDisabled(_screenSettingsManager.Fullscreen);

            _resolutionButton.SetFocusDisabled(_screenSettingsManager.Fullscreen);
            UpdateResolutionButton();

            HideSettingsMenu();
        }

        private void ConfigureScreenSettingsButtons() {
            _fullscreenButtonWrapper
                .OnFocusEntered(() => {
                    _scrollContainer.ScrollVertical = 0;
                    _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                });
            _fullscreenButtonWrapper.OnToggled(isChecked => {
                _resolutionButton.SetFocusDisabled(isChecked);
                _borderlessButtonWrapper.SetFocusDisabled(isChecked);
                if (isChecked) {
                    _borderlessButtonWrapper.Pressed = false;
                }
                _screenSettingsManager.SetFullscreen(isChecked);
                CheckIfResolutionStillMatches();
            });
            _resolutionButton.OnFocusEntered(() => {
                UpdateResolutionButton();
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsResolution();
            });
            _resolutionButton.OnFocusExited(UpdateResolutionButton);
            _pixelPerfectButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _pixelPerfectButtonWrapper.OnToggled(isChecked => {
                _screenSettingsManager.SetPixelPerfect(isChecked);
                CheckIfResolutionStillMatches();
            });

            _borderlessButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _borderlessButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetBorderless(isChecked));

            _vsyncButtonWrapper.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            _vsyncButtonWrapper.OnToggled(isChecked => _screenSettingsManager.SetVSync(isChecked));
        }

        private void ConfigureControls() {
            // Remove all
            foreach (Node child in _gamepadControls.GetChildren()) child.QueueFree();
            foreach (Node child in _keyboardControls.GetChildren()) child.QueueFree();
            
            // TODO: i18n
            AddConfigureControl("Jump", Jump, false);
            AddConfigureControl("Attack", Attack, false);
            
            AddConfigureControl("Up", Up, true);
            AddConfigureControl("Down", Down, true);
            AddConfigureControl("Left", Left, true);
            AddConfigureControl("Right", Right, true);
            AddConfigureControl("Jump", Jump, true);
            AddConfigureControl("Attack", Attack, true);
            
            _keyboardControls.GetChild<Button>(_gamepadControls.GetChildCount() - 1).OnFocusEntered(() => {
                _gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack();
                _scrollContainer.ScrollVertical = int.MaxValue;
            });
        }

        private void AddConfigureControl(string name, InputAction action, bool isKey) {
            var button = MainResourceLoader.RedefineActionButtonFactory();
            button.OnPressed(() => ShowRedefineActionPanel(button));
            button.OnFocusEntered(_gameManager.MainMenuBottomBarScene.ConfigureSettingsChangeBack);
            button.SetInputAction(name, action, isKey);
            if (isKey) _keyboardControls.AddChild(button);
            else _gamepadControls.AddChild(button);
        } 

        private Tuple<ScaledResolution, List<ScaledResolution>, int> FindClosestResolutionToSelected() {
            List<ScaledResolution> resolutions = _screenSettingsManager.GetResolutions();
            Resolution currentResolution = _screenSettingsManager.WindowedResolution;
            var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == currentResolution.Size);
            if (pos == -1) {
                // Find the closest resolution with the same or smaller height
                pos = resolutions.Count(scaledResolution => scaledResolution.Size.y <= currentResolution.Size.y) - 1;
                if (pos == -1) pos = 0;
            }
            return new Tuple<ScaledResolution, List<ScaledResolution>, int>(resolutions[pos], resolutions, pos);
        }

        private void CheckIfResolutionStillMatches() {
            if (_screenSettingsManager.IsFullscreen()) return; 
            var (closestResolution, resolutions, pos) = FindClosestResolutionToSelected();
            if (_screenSettingsManager.WindowedResolution.Size != closestResolution.Size) {
                _screenSettingsManager.SetWindowed(resolutions[pos]);
                UpdateResolutionButton();
            }
        }

        private bool ProcessChangeResolution(InputEvent e) {
            if (!UiLeft.IsEventJustPressed(e) && !UiRight.IsEventJustPressed(e) &&
                !UiAccept.IsEventJustPressed(e)) return false;
            var (_, resolutions, pos) = FindClosestResolutionToSelected();
            if (UiLeft.IsEventJustPressed(e)) {
                if (pos > 0) {
                    _screenSettingsManager.SetWindowed(resolutions[pos - 1]);
                    UpdateResolutionButton();
                }
            } else if (UiRight.IsEventJustPressed(e)) {
                if (pos < resolutions.Count - 1) {
                    _screenSettingsManager.SetWindowed(resolutions[pos + 1]);
                    UpdateResolutionButton();
                }
            } else if (UiAccept.IsEventJustPressed(e)) {
                _screenSettingsManager.SetWindowed(pos == resolutions.Count - 1
                    ? resolutions[0]
                    : resolutions[pos + 1]);
                UpdateResolutionButton();
            }
            return true;
        }

        private void UpdateResolutionButton() {
            var (scaledResolution, resolutions, pos) = FindClosestResolutionToSelected();
            var prefix = pos > 0 ? "< " : "";
            var suffix = pos < resolutions.Count - 1 ? " >" : "";
            var res = scaledResolution.ToString();
            if (scaledResolution.Size == _screenSettingsManager.InitialScreenConfiguration.BaseResolution.Size) {
                res += " (Original)";
            } else if (scaledResolution.Base == _screenSettingsManager.InitialScreenConfiguration.BaseResolution.Size &&
                       scaledResolution.IsPixelPerfectScale()) {
                res += " (x" + scaledResolution.GetPixelPerfectScale() + ")";
            }
            _resolutionButton.Text = prefix + res + suffix;
        }

        public void OnInput(InputEvent e) {
            // if (e.IsAnyKey()) {
                // Console.WriteLine("GetKeyString:" + e.GetKeyString() + " / Enum:" + e.GetKey()+" / Unicode: "+e.GetKeyUnicode());
            // } else if (e.IsAnyButton()) {
                // Console.WriteLine("ButtonString:" + e.GetButtonString() + " / Enum:" + e.GetButton());
            // }
            if (IsWaitingFromRedefineInputEvent()) {
                RedefineControlFromInputEvent(e);
                GetTree().SetInputAsHandled();
                
            } else if (UiCancel.IsEventPressed(e)) {
                _gameManager.TriggerBack();
                GetTree().SetInputAsHandled();
                
            } else if (_resolutionButton.HasFocus()) {
                if (ProcessChangeResolution(e)) {
                    GetTree().SetInputAsHandled();
                }
            }
        }

        private RedefineActionButton? _redefineButtonSelected;

        public bool IsWaitingFromRedefineInputEvent() {
            return _redefineButtonSelected != null;
        }

        public void ShowRedefineActionPanel(RedefineActionButton button) {
            _redefineButtonSelected = button;
            _redefineBox.Show();
            _settingsBox.Hide();
            _redefineActionName.Text = button.ActionName;
            // TODO: i18n
            _redefineActionMessage.Text = button.IsKey ? "Press key for..." : "Press button for...";
            _gameManager.MainMenuBottomBarScene.HideAll();
        }

        private void RedefineControlFromInputEvent(InputEvent e) {
            if (!e.IsKey(KeyList.Escape)) {
                if (_redefineButtonSelected!.IsKey && e.IsAnyKey() && !e.IsKey(KeyList.Escape)) {
                    _redefineButtonSelected!.InputAction.ClearKeys().AddKey(e.GetKey()).Save().Setup();
                    _redefineButtonSelected.Refresh();
                } else if (_redefineButtonSelected!.IsButton && e.IsAnyButton()) {
                    _redefineButtonSelected!.InputAction.ClearButtons().AddButton(e.GetButton()).Save().Setup();
                    _redefineButtonSelected.Refresh();
                } else {
                    // Ignore the event
                    return;
                }
            }
            _redefineBox.Hide();
            _settingsBox.Show();
            _redefineButtonSelected!.GrabFocus();
            _redefineButtonSelected = null;
        }

        public void ShowSettingsMenu() {
            _panel.Show();
            _settingsBox.Show();
            _redefineBox.Hide();
            _fullscreenButtonWrapper.GrabFocus();
        }

        public void HideSettingsMenu() {
            _launcher.RemoveAll();
            _panel.Hide();
        }
    }
}