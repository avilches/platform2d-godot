using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class VehicleBodyAction : ProxyNode {

        private List<Action<Node>>? _onBodyEnteredAction; 
        public VehicleBodyAction OnBodyEntered(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnBodyEntered(Action<Node> action) {
            RemoveSignal(_onBodyEnteredAction, "body_entered", nameof(_GodotSignalBodyEntered), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalBodyEntered(Node body) {
            ExecuteSignal(_onBodyEnteredAction, body);
            return this;
        }

        private List<Action<Node>>? _onBodyExitedAction; 
        public VehicleBodyAction OnBodyExited(Action<Node> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnBodyExited(Action<Node> action) {
            RemoveSignal(_onBodyExitedAction, "body_exited", nameof(_GodotSignalBodyExited), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalBodyExited(Node body) {
            ExecuteSignal(_onBodyExitedAction, body);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeEnteredAction; 
        public VehicleBodyAction OnBodyShapeEntered(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnBodyShapeEntered(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeEnteredAction, "body_shape_entered", nameof(_GodotSignalBodyShapeEntered), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalBodyShapeEntered(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeEnteredAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action<Node, RID, int, int>>? _onBodyShapeExitedAction; 
        public VehicleBodyAction OnBodyShapeExited(Action<Node, RID, int, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnBodyShapeExited(Action<Node, RID, int, int> action) {
            RemoveSignal(_onBodyShapeExitedAction, "body_shape_exited", nameof(_GodotSignalBodyShapeExited), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalBodyShapeExited(Node body, RID body_rid, int body_shape_index, int local_shape_index) {
            ExecuteSignal(_onBodyShapeExitedAction, body, body_rid, body_shape_index, local_shape_index);
            return this;
        }

        private List<Action>? _onGameplayEnteredAction; 
        public VehicleBodyAction OnGameplayEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnGameplayEntered(Action action) {
            RemoveSignal(_onGameplayEnteredAction, "gameplay_entered", nameof(_GodotSignalGameplayEntered), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalGameplayEntered() {
            ExecuteSignal(_onGameplayEnteredAction);
            return this;
        }

        private List<Action>? _onGameplayExitedAction; 
        public VehicleBodyAction OnGameplayExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnGameplayExited(Action action) {
            RemoveSignal(_onGameplayExitedAction, "gameplay_exited", nameof(_GodotSignalGameplayExited), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalGameplayExited() {
            ExecuteSignal(_onGameplayExitedAction);
            return this;
        }

        private List<Action<InputEvent, Node, Vector3, Vector3, int>>? _onInputEventAction; 
        public VehicleBodyAction OnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnInputEvent(Action<InputEvent, Node, Vector3, Vector3, int> action) {
            RemoveSignal(_onInputEventAction, "input_event", nameof(_GodotSignalInputEvent), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalInputEvent(InputEvent @event, Node camera, Vector3 normal, Vector3 position, int shape_idx) {
            ExecuteSignal(_onInputEventAction, @event, camera, normal, position, shape_idx);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public VehicleBodyAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public VehicleBodyAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public VehicleBodyAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public VehicleBodyAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VehicleBodyAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSleepingStateChangedAction; 
        public VehicleBodyAction OnSleepingStateChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSleepingStateChangedAction, "sleeping_state_changed", nameof(_GodotSignalSleepingStateChanged), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnSleepingStateChanged(Action action) {
            RemoveSignal(_onSleepingStateChangedAction, "sleeping_state_changed", nameof(_GodotSignalSleepingStateChanged), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalSleepingStateChanged() {
            ExecuteSignal(_onSleepingStateChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public VehicleBodyAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public VehicleBodyAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public VehicleBodyAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public VehicleBodyAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public VehicleBodyAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private VehicleBodyAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}