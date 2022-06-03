using System;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Godot;

namespace Veronenger.Game.Controller.UI {
    public class ButtonWrapper : DiButton {
        public class Context {
            public ButtonWrapper Button { get; }

            public Context(ButtonWrapper button) {
                Button = button;
            }
        }

        public class InputEventContext : Context {
            public InputEvent InputEvent { get; }

            public InputEventContext(ButtonWrapper button, InputEvent @event) : base(button) {
                InputEvent = @event;
            }
        }

        private readonly ControlRestorer _saver;
        private Action<bool>? _onPressedAction;
        private Action<Context>? _onPressedActionWithContext;
        private Func<InputEventContext, bool>? _onInputEvent;
        private Action? _onFocusEntered;
        private Action? _onFocusExited;

        // TODO: i18n
        internal ButtonWrapper() {
            _saver = new ControlRestorer(this);
            Connect(GodotConstants.GODOT_SIGNAL_pressed, this, nameof(_GodotPressedSignal));
        }

        public void Save() => _saver.Save();
        public void Restore() => _saver.Restore();

        public override void _Input(InputEvent @event) {
            // It takes into account if the Root.GuiDisableInput = true
            if (_onInputEvent != null && GetFocusOwner() == this && !Disabled) {
                if (_onInputEvent(new InputEventContext(this, @event))) {
                    GetTree().SetInputAsHandled();
                }
            }
        }

        private void _GodotPressedSignal() {
            if (_onPressedActionWithContext != null) _onPressedActionWithContext(new Context(this));
            else _onPressedAction?.Invoke(Pressed);
        }

        private void _GodotFocusEnteredSignal() => _onFocusEntered?.Invoke();
        private void _GodotFocusExitedSignal() => _onFocusExited?.Invoke();

        public ButtonWrapper OnPressed(Action<bool>? onPressedAction) {
            _onPressedAction = onPressedAction;
            _onPressedActionWithContext = null;
            return this;
        }

        public ButtonWrapper OnPressed(Action<Context>? onPressedActionWithContext) {
            _onPressedAction = null;
            _onPressedActionWithContext = onPressedActionWithContext;
            return this;
        }

        public ButtonWrapper OnInputEvent(Func<InputEventContext, bool>? onPressedActionWithInputEventContext) {
            _onPressedAction = null;
            _onPressedActionWithContext = null;
            _onInputEvent = onPressedActionWithInputEventContext;
            return this;
        }

        public ButtonWrapper OnFocusEntered(Action onFocus) {
            if (_onFocusEntered == null) 
                Connect(GodotConstants.GODOT_SIGNAL_focus_entered, this, nameof(_GodotFocusEnteredSignal));
            _onFocusEntered = onFocus;
            return this;
        }

        public ButtonWrapper OnFocusExited(Action onFocus) {
            if (_onFocusExited == null) 
                Connect(GodotConstants.GODOT_SIGNAL_focus_exited, this, nameof(_GodotFocusExitedSignal));
            _onFocusExited = onFocus;
            return this;
        }
    }
}