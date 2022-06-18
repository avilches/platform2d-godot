using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class Area2DAction : Area2D {

        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public Area2DAction OnProcess(Action<float> action) {
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }
        public Area2DAction OnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }

        public Area2DAction OnInput(Action<InputEvent> action) {
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }

        public Area2DAction OnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }

        public Area2DAction OnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }

        public Area2DAction RemoveOnProcess(Action<float> action) {
            _onProcessActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnPhysicsProcess(Action<float> action) {
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnInput(Action<InputEvent> action) {
            _onInputActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnUnhandledInput(Action<InputEvent> action) {
            _onUnhandledInputActions?.Remove(action);
            return this;
        }

        public Area2DAction RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {
            _onUnhandledKeyInputActions?.Remove(action);
            return this;
        }

        public override void _Process(float delta) {
            if (_onProcessActions == null) {
                SetProcess(false);
                return;
            }
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }

        public override void _PhysicsProcess(float delta) {
            if (_onPhysicsProcessActions == null) {
                SetPhysicsProcess(true);
                return;
            }
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }

        public override void _Input(InputEvent @event) {
            if (_onInputActions == null) {
                SetProcessInput(true);
                return;
            }
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }

        public override void _UnhandledInput(InputEvent @event) {
            if (_onUnhandledInputActions == null) {
                SetProcessUnhandledInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }

        public override void _UnhandledKeyInput(InputEventKey @event) {
            if (_onUnhandledKeyInputActions == null) {
                SetProcessUnhandledKeyInput(true);
                return;
            }
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }

        private Action<Area2D>? _onAreaEnteredAction; 
        public Area2DAction OnAreaEntered(Action<Area2D> action) {
            if (_onAreaEnteredAction == null) 
                Connect("area_entered", this, nameof(ExecuteAreaEntered));
            _onAreaEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnAreaEntered() {
            if (_onAreaEnteredAction == null) return this; 
            Disconnect("area_entered", this, nameof(ExecuteAreaEntered));
            _onAreaEnteredAction = null;
            return this;
        }
        private void ExecuteAreaEntered(Area2D area) =>
            _onAreaEnteredAction?.Invoke(area);
        

        private Action<Area2D>? _onAreaExitedAction; 
        public Area2DAction OnAreaExited(Action<Area2D> action) {
            if (_onAreaExitedAction == null) 
                Connect("area_exited", this, nameof(ExecuteAreaExited));
            _onAreaExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnAreaExited() {
            if (_onAreaExitedAction == null) return this; 
            Disconnect("area_exited", this, nameof(ExecuteAreaExited));
            _onAreaExitedAction = null;
            return this;
        }
        private void ExecuteAreaExited(Area2D area) =>
            _onAreaExitedAction?.Invoke(area);
        

        private Action<Area2D, RID, int, int>? _onAreaShapeEnteredAction; 
        public Area2DAction OnAreaShapeEntered(Action<Area2D, RID, int, int> action) {
            if (_onAreaShapeEnteredAction == null) 
                Connect("area_shape_entered", this, nameof(ExecuteAreaShapeEntered));
            _onAreaShapeEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnAreaShapeEntered() {
            if (_onAreaShapeEnteredAction == null) return this; 
            Disconnect("area_shape_entered", this, nameof(ExecuteAreaShapeEntered));
            _onAreaShapeEnteredAction = null;
            return this;
        }
        private void ExecuteAreaShapeEntered(Area2D area, RID area_rid, int area_shape_index, int local_shape_index) =>
            _onAreaShapeEnteredAction?.Invoke(area, area_rid, area_shape_index, local_shape_index);
        

        private Action<Area2D, RID, int, int>? _onAreaShapeExitedAction; 
        public Area2DAction OnAreaShapeExited(Action<Area2D, RID, int, int> action) {
            if (_onAreaShapeExitedAction == null) 
                Connect("area_shape_exited", this, nameof(ExecuteAreaShapeExited));
            _onAreaShapeExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnAreaShapeExited() {
            if (_onAreaShapeExitedAction == null) return this; 
            Disconnect("area_shape_exited", this, nameof(ExecuteAreaShapeExited));
            _onAreaShapeExitedAction = null;
            return this;
        }
        private void ExecuteAreaShapeExited(Area2D area, RID area_rid, int area_shape_index, int local_shape_index) =>
            _onAreaShapeExitedAction?.Invoke(area, area_rid, area_shape_index, local_shape_index);
        

        private Action<Node>? _onBodyEnteredAction; 
        public Area2DAction OnBodyEntered(Action<Node> action) {
            if (_onBodyEnteredAction == null) 
                Connect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnBodyEntered() {
            if (_onBodyEnteredAction == null) return this; 
            Disconnect("body_entered", this, nameof(ExecuteBodyEntered));
            _onBodyEnteredAction = null;
            return this;
        }
        private void ExecuteBodyEntered(Node body) =>
            _onBodyEnteredAction?.Invoke(body);
        

        private Action<Node>? _onBodyExitedAction; 
        public Area2DAction OnBodyExited(Action<Node> action) {
            if (_onBodyExitedAction == null) 
                Connect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnBodyExited() {
            if (_onBodyExitedAction == null) return this; 
            Disconnect("body_exited", this, nameof(ExecuteBodyExited));
            _onBodyExitedAction = null;
            return this;
        }
        private void ExecuteBodyExited(Node body) =>
            _onBodyExitedAction?.Invoke(body);
        

        private Action<Node, RID, int, int>? _onBodyShapeEnteredAction; 
        public Area2DAction OnBodyShapeEntered(Action<Node, RID, int, int> action) {
            if (_onBodyShapeEnteredAction == null) 
                Connect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnBodyShapeEntered() {
            if (_onBodyShapeEnteredAction == null) return this; 
            Disconnect("body_shape_entered", this, nameof(ExecuteBodyShapeEntered));
            _onBodyShapeEnteredAction = null;
            return this;
        }
        private void ExecuteBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeEnteredAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action<Node, RID, int, int>? _onBodyShapeExitedAction; 
        public Area2DAction OnBodyShapeExited(Action<Node, RID, int, int> action) {
            if (_onBodyShapeExitedAction == null) 
                Connect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnBodyShapeExited() {
            if (_onBodyShapeExitedAction == null) return this; 
            Disconnect("body_shape_exited", this, nameof(ExecuteBodyShapeExited));
            _onBodyShapeExitedAction = null;
            return this;
        }
        private void ExecuteBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            _onBodyShapeExitedAction?.Invoke(body, body_rid, body_shape_index, local_shape_index);
        

        private Action? _onDrawAction; 
        public Area2DAction OnDraw(Action action) {
            if (_onDrawAction == null) 
                Connect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = action;
            return this;
        }
        public Area2DAction RemoveOnDraw() {
            if (_onDrawAction == null) return this; 
            Disconnect("draw", this, nameof(ExecuteDraw));
            _onDrawAction = null;
            return this;
        }
        private void ExecuteDraw() =>
            _onDrawAction?.Invoke();
        

        private Action? _onHideAction; 
        public Area2DAction OnHide(Action action) {
            if (_onHideAction == null) 
                Connect("hide", this, nameof(ExecuteHide));
            _onHideAction = action;
            return this;
        }
        public Area2DAction RemoveOnHide() {
            if (_onHideAction == null) return this; 
            Disconnect("hide", this, nameof(ExecuteHide));
            _onHideAction = null;
            return this;
        }
        private void ExecuteHide() =>
            _onHideAction?.Invoke();
        

        private Action<InputEvent, int, Node>? _onInputEventAction; 
        public Area2DAction OnInputEvent(Action<InputEvent, int, Node> action) {
            if (_onInputEventAction == null) 
                Connect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = action;
            return this;
        }
        public Area2DAction RemoveOnInputEvent() {
            if (_onInputEventAction == null) return this; 
            Disconnect("input_event", this, nameof(ExecuteInputEvent));
            _onInputEventAction = null;
            return this;
        }
        private void ExecuteInputEvent(InputEvent @event, int shape_idx, Node viewport) =>
            _onInputEventAction?.Invoke(@event, shape_idx, viewport);
        

        private Action? _onItemRectChangedAction; 
        public Area2DAction OnItemRectChanged(Action action) {
            if (_onItemRectChangedAction == null) 
                Connect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = action;
            return this;
        }
        public Area2DAction RemoveOnItemRectChanged() {
            if (_onItemRectChangedAction == null) return this; 
            Disconnect("item_rect_changed", this, nameof(ExecuteItemRectChanged));
            _onItemRectChangedAction = null;
            return this;
        }
        private void ExecuteItemRectChanged() =>
            _onItemRectChangedAction?.Invoke();
        

        private Action? _onMouseEnteredAction; 
        public Area2DAction OnMouseEntered(Action action) {
            if (_onMouseEnteredAction == null) 
                Connect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnMouseEntered() {
            if (_onMouseEnteredAction == null) return this; 
            Disconnect("mouse_entered", this, nameof(ExecuteMouseEntered));
            _onMouseEnteredAction = null;
            return this;
        }
        private void ExecuteMouseEntered() =>
            _onMouseEnteredAction?.Invoke();
        

        private Action? _onMouseExitedAction; 
        public Area2DAction OnMouseExited(Action action) {
            if (_onMouseExitedAction == null) 
                Connect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnMouseExited() {
            if (_onMouseExitedAction == null) return this; 
            Disconnect("mouse_exited", this, nameof(ExecuteMouseExited));
            _onMouseExitedAction = null;
            return this;
        }
        private void ExecuteMouseExited() =>
            _onMouseExitedAction?.Invoke();
        

        private Action? _onReadyAction; 
        public Area2DAction OnReady(Action action) {
            if (_onReadyAction == null) 
                Connect("ready", this, nameof(ExecuteReady));
            _onReadyAction = action;
            return this;
        }
        public Area2DAction RemoveOnReady() {
            if (_onReadyAction == null) return this; 
            Disconnect("ready", this, nameof(ExecuteReady));
            _onReadyAction = null;
            return this;
        }
        private void ExecuteReady() =>
            _onReadyAction?.Invoke();
        

        private Action? _onRenamedAction; 
        public Area2DAction OnRenamed(Action action) {
            if (_onRenamedAction == null) 
                Connect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = action;
            return this;
        }
        public Area2DAction RemoveOnRenamed() {
            if (_onRenamedAction == null) return this; 
            Disconnect("renamed", this, nameof(ExecuteRenamed));
            _onRenamedAction = null;
            return this;
        }
        private void ExecuteRenamed() =>
            _onRenamedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public Area2DAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public Area2DAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeEnteredAction; 
        public Area2DAction OnTreeEntered(Action action) {
            if (_onTreeEnteredAction == null) 
                Connect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = action;
            return this;
        }
        public Area2DAction RemoveOnTreeEntered() {
            if (_onTreeEnteredAction == null) return this; 
            Disconnect("tree_entered", this, nameof(ExecuteTreeEntered));
            _onTreeEnteredAction = null;
            return this;
        }
        private void ExecuteTreeEntered() =>
            _onTreeEnteredAction?.Invoke();
        

        private Action? _onTreeExitedAction; 
        public Area2DAction OnTreeExited(Action action) {
            if (_onTreeExitedAction == null) 
                Connect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = action;
            return this;
        }
        public Area2DAction RemoveOnTreeExited() {
            if (_onTreeExitedAction == null) return this; 
            Disconnect("tree_exited", this, nameof(ExecuteTreeExited));
            _onTreeExitedAction = null;
            return this;
        }
        private void ExecuteTreeExited() =>
            _onTreeExitedAction?.Invoke();
        

        private Action? _onTreeExitingAction; 
        public Area2DAction OnTreeExiting(Action action) {
            if (_onTreeExitingAction == null) 
                Connect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = action;
            return this;
        }
        public Area2DAction RemoveOnTreeExiting() {
            if (_onTreeExitingAction == null) return this; 
            Disconnect("tree_exiting", this, nameof(ExecuteTreeExiting));
            _onTreeExitingAction = null;
            return this;
        }
        private void ExecuteTreeExiting() =>
            _onTreeExitingAction?.Invoke();
        

        private Action? _onVisibilityChangedAction; 
        public Area2DAction OnVisibilityChanged(Action action) {
            if (_onVisibilityChangedAction == null) 
                Connect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = action;
            return this;
        }
        public Area2DAction RemoveOnVisibilityChanged() {
            if (_onVisibilityChangedAction == null) return this; 
            Disconnect("visibility_changed", this, nameof(ExecuteVisibilityChanged));
            _onVisibilityChangedAction = null;
            return this;
        }
        private void ExecuteVisibilityChanged() =>
            _onVisibilityChangedAction?.Invoke();
        
    }
}