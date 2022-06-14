using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisibilityNotifier2DAction : VisibilityNotifier2D {

        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public VisibilityNotifier2DAction OnProcessAction(Action<float> action) {
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }
        public VisibilityNotifier2DAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public VisibilityNotifier2DAction OnInput(Action<InputEvent> action) {
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }

        public VisibilityNotifier2DAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public VisibilityNotifier2DAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public VisibilityNotifier2DAction RemoveOnProcessAction(Action<float> action) {
            _onProcessAction?.Remove(action);
            return this;
        }

        public VisibilityNotifier2DAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcess?.Remove(action);
            return this;
        }

        public VisibilityNotifier2DAction RemoveOnInput(Action<InputEvent> action) {
            _onInput?.Remove(action);
            return this;
        }

        public VisibilityNotifier2DAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInput?.Remove(action);
            return this;
        }

        public VisibilityNotifier2DAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInput?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessAction == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessAction.Count; i++) _onProcessAction[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcess == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcess.Count; i++) _onPhysicsProcess[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInput == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInput.Count; i++) _onInput[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInput == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInput.Count; i++) _onUnhandledInput[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInput == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInput.Count; i++) _onUnhandledKeyInput[i].Invoke(@event);
        }

        private Action? _onDrawAction; 
        public VisibilityNotifier2DAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onHideAction; 
        public VisibilityNotifier2DAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action? _onItemRectChangedAction; 
        public VisibilityNotifier2DAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public VisibilityNotifier2DAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public VisibilityNotifier2DAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScreenEnteredAction; 
        public VisibilityNotifier2DAction OnScreenEntered(Action action) {
            if (_onScreenEnteredAction == null) 
                Connect("screen_entered", this, nameof(ExecuteScreenEntered));
            _onScreenEnteredAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnScreenEntered() {
            if (_onScreenEnteredAction == null) return this; 
            Disconnect("screen_entered", this, nameof(ExecuteScreenEntered));
            _onScreenEnteredAction = null;
            return this;
        }
        private void ExecuteScreenEntered() =>
            _onScreenEnteredAction?.Invoke();
        

        private Action? _onScreenExitedAction; 
        public VisibilityNotifier2DAction OnScreenExited(Action action) {
            if (_onScreenExitedAction == null) 
                Connect("screen_exited", this, nameof(ExecuteScreenExited));
            _onScreenExitedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnScreenExited() {
            if (_onScreenExitedAction == null) return this; 
            Disconnect("screen_exited", this, nameof(ExecuteScreenExited));
            _onScreenExitedAction = null;
            return this;
        }
        private void ExecuteScreenExited() =>
            _onScreenExitedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public VisibilityNotifier2DAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public VisibilityNotifier2DAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public VisibilityNotifier2DAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public VisibilityNotifier2DAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action<Viewport>? _onViewportEnteredAction; 
        public VisibilityNotifier2DAction OnViewportEntered(Action<Viewport> action) {
            if (_onViewportEnteredAction == null) 
                Connect("viewport_entered", this, nameof(ExecuteViewportEntered));
            _onViewportEnteredAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnViewportEntered() {
            if (_onViewportEnteredAction == null) return this; 
            Disconnect("viewport_entered", this, nameof(ExecuteViewportEntered));
            _onViewportEnteredAction = null;
            return this;
        }
        private void ExecuteViewportEntered(Viewport viewport) =>
            _onViewportEnteredAction?.Invoke(viewport);
        

        private Action<Viewport>? _onViewportExitedAction; 
        public VisibilityNotifier2DAction OnViewportExited(Action<Viewport> action) {
            if (_onViewportExitedAction == null) 
                Connect("viewport_exited", this, nameof(ExecuteViewportExited));
            _onViewportExitedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnViewportExited() {
            if (_onViewportExitedAction == null) return this; 
            Disconnect("viewport_exited", this, nameof(ExecuteViewportExited));
            _onViewportExitedAction = null;
            return this;
        }
        private void ExecuteViewportExited(Viewport viewport) =>
            _onViewportExitedAction?.Invoke(viewport);
        

        private Action? _onVisibilityChangedAction; 
        public VisibilityNotifier2DAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public VisibilityNotifier2DAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}