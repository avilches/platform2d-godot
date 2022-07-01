using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AreaAction : ProxyNode {

        private List<Action<Area>>? _onAreaEnteredAction; 
        public void OnAreaEntered(Action<Area> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAreaEnteredAction, "area_entered", nameof(_GodotSignalAreaEntered), action, oneShot, deferred);

        public void RemoveOnAreaEntered(Action<Area> action) =>
            RemoveSignal(_onAreaEnteredAction, "area_entered", nameof(_GodotSignalAreaEntered), action);

        private void _GodotSignalAreaEntered(Area area) =>
            ExecuteSignal(_onAreaEnteredAction, area);
        

        private List<Action<Area>>? _onAreaExitedAction; 
        public void OnAreaExited(Action<Area> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAreaExitedAction, "area_exited", nameof(_GodotSignalAreaExited), action, oneShot, deferred);

        public void RemoveOnAreaExited(Action<Area> action) =>
            RemoveSignal(_onAreaExitedAction, "area_exited", nameof(_GodotSignalAreaExited), action);

        private void _GodotSignalAreaExited(Area area) =>
            ExecuteSignal(_onAreaExitedAction, area);
        

        private List<Action<Area, RID, int, int>>? _onAreaShapeEnteredAction; 
        public void OnAreaShapeEntered(Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAreaShapeEnteredAction, "area_shape_entered", nameof(_GodotSignalAreaShapeEntered), action, oneShot, deferred);

        public void RemoveOnAreaShapeEntered(Action<Area, RID, int, int> action) =>
            RemoveSignal(_onAreaShapeEnteredAction, "area_shape_entered", nameof(_GodotSignalAreaShapeEntered), action);

        private void _GodotSignalAreaShapeEntered(Area area, RID area_rid, int area_shape_index, int local_shape_index) =>
            ExecuteSignal(_onAreaShapeEnteredAction, area, area_rid, area_shape_index, local_shape_index);
        

        private List<Action<Area, RID, int, int>>? _onAreaShapeExitedAction; 
        public void OnAreaShapeExited(Action<Area, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onAreaShapeExitedAction, "area_shape_exited", nameof(_GodotSignalAreaShapeExited), action, oneShot, deferred);

        public void RemoveOnAreaShapeExited(Action<Area, RID, int, int> action) =>
            RemoveSignal(_onAreaShapeExitedAction, "area_shape_exited", nameof(_GodotSignalAreaShapeExited), action);

        private void _GodotSignalAreaShapeExited(Area area, RID area_rid, int area_shape_index, int local_shape_index) =>
            ExecuteSignal(_onAreaShapeExitedAction, area, area_rid, area_shape_index, local_shape_index);
        

        private List<Action<Node>>? _onBodyEnteredAction; 
        public void OnBodyEntered(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action, oneShot, deferred);

        public void RemoveOnBodyEntered(Action<Node> action) =>
            RemoveSignal(_onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action);

        private void _GodotSignalBodyEntered(Node body) =>
            ExecuteSignal(_onBodyEnteredAction, body);
        

        private List<Action<Node>>? _onBodyExitedAction; 
        public void OnBodyExited(Action<Node> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action, oneShot, deferred);

        public void RemoveOnBodyExited(Action<Node> action) =>
            RemoveSignal(_onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action);

        private void _GodotSignalBodyExited(Node body) =>
            ExecuteSignal(_onBodyExitedAction, body);
        

        private List<Action<Node, RID, int, int>>? _onBodyShapeEnteredAction; 
        public void OnBodyShapeEntered(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action, oneShot, deferred);

        public void RemoveOnBodyShapeEntered(Action<Node, RID, int, int> action) =>
            RemoveSignal(_onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action);

        private void _GodotSignalBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            ExecuteSignal(_onBodyShapeEnteredAction, body, body_rid, body_shape_index, local_shape_index);
        

        private List<Action<Node, RID, int, int>>? _onBodyShapeExitedAction; 
        public void OnBodyShapeExited(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action, oneShot, deferred);

        public void RemoveOnBodyShapeExited(Action<Node, RID, int, int> action) =>
            RemoveSignal(_onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action);

        private void _GodotSignalBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) =>
            ExecuteSignal(_onBodyShapeExitedAction, body, body_rid, body_shape_index, local_shape_index);
        

        private List<Action>? _onGameplayEnteredAction; 
        public void OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);

        public void RemoveOnGameplayEntered(Action action) =>
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);

        private void _GodotSignalGameplayEntered() =>
            ExecuteSignal(_onGameplayEnteredAction);
        

        private List<Action>? _onGameplayExitedAction; 
        public void OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);

        public void RemoveOnGameplayExited(Action action) =>
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);

        private void _GodotSignalGameplayExited() =>
            ExecuteSignal(_onGameplayExitedAction);
        

        private List<Action<InputEvent, Node, Vector3, Vector3, int>>? _onInputEventAction; 
        public void OnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action, oneShot, deferred);

        public void RemoveOnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action) =>
            RemoveSignal(_onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action);

        private void _GodotSignalInputEvent(InputEvent @event, Node camera, Vector3 normal, Vector3 position, int shape_idx) =>
            ExecuteSignal(_onInputEventAction, @event, camera, normal, position, shape_idx);
        

        private List<Action>? _onMouseEnteredAction; 
        public void OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);

        public void RemoveOnMouseEntered(Action action) =>
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);

        private void _GodotSignalMouseEntered() =>
            ExecuteSignal(_onMouseEnteredAction);
        

        private List<Action>? _onMouseExitedAction; 
        public void OnMouseExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);

        public void RemoveOnMouseExited(Action action) =>
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);

        private void _GodotSignalMouseExited() =>
            ExecuteSignal(_onMouseExitedAction);
        

        private List<Action>? _onReadyAction; 
        public void OnReady(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);

        public void RemoveOnReady(Action action) =>
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);

        private void _GodotSignalReady() =>
            ExecuteSignal(_onReadyAction);
        

        private List<Action>? _onRenamedAction; 
        public void OnRenamed(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);

        public void RemoveOnRenamed(Action action) =>
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);

        private void _GodotSignalRenamed() =>
            ExecuteSignal(_onRenamedAction);
        

        private List<Action>? _onScriptChangedAction; 
        public void OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);

        public void RemoveOnScriptChanged(Action action) =>
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);

        private void _GodotSignalScriptChanged() =>
            ExecuteSignal(_onScriptChangedAction);
        

        private List<Action>? _onTreeEnteredAction; 
        public void OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);

        public void RemoveOnTreeEntered(Action action) =>
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);

        private void _GodotSignalTreeEntered() =>
            ExecuteSignal(_onTreeEnteredAction);
        

        private List<Action>? _onTreeExitedAction; 
        public void OnTreeExited(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);

        public void RemoveOnTreeExited(Action action) =>
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);

        private void _GodotSignalTreeExited() =>
            ExecuteSignal(_onTreeExitedAction);
        

        private List<Action>? _onTreeExitingAction; 
        public void OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);

        public void RemoveOnTreeExiting(Action action) =>
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);

        private void _GodotSignalTreeExiting() =>
            ExecuteSignal(_onTreeExitingAction);
        

        private List<Action>? _onVisibilityChangedAction; 
        public void OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) =>
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);

        public void RemoveOnVisibilityChanged(Action action) =>
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);

        private void _GodotSignalVisibilityChanged() =>
            ExecuteSignal(_onVisibilityChangedAction);
        
    }
}