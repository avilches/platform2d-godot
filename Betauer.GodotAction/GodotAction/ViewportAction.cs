using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class ViewportAction : ProxyNode {

        private List<Action<Control>>? _onGuiFocusChangedAction; 
        public ViewportAction OnGuiFocusChanged(Action<Control> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiFocusChangedAction, "gui_focus_changed", nameof(_GodotSignalGuiFocusChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnGuiFocusChanged(Action<Control> action) {
            RemoveSignal(_onGuiFocusChangedAction, "gui_focus_changed", nameof(_GodotSignalGuiFocusChanged), action);
            return this;
        }

        private ViewportAction _GodotSignalGuiFocusChanged(Control node) {
            ExecuteSignal(_onGuiFocusChangedAction, node);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ViewportAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ViewportAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ViewportAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ViewportAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ViewportAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ViewportAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onSizeChangedAction; 
        public ViewportAction OnSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeChangedAction, "size_changed", nameof(_GodotSignalSizeChanged), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnSizeChanged(Action action) {
            RemoveSignal(_onSizeChangedAction, "size_changed", nameof(_GodotSignalSizeChanged), action);
            return this;
        }

        private ViewportAction _GodotSignalSizeChanged() {
            ExecuteSignal(_onSizeChangedAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ViewportAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ViewportAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ViewportAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ViewportAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ViewportAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ViewportAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ViewportAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }
    }
}