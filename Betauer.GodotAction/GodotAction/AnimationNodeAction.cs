using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public AnimationNodeAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private AnimationNodeAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeAction OnRemovedFromGraph(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeAction RemoveOnRemovedFromGraph(Action action) {
            RemoveSignal(_onRemovedFromGraphAction, "removed_from_graph", nameof(_GodotSignalRemovedFromGraph), action);
            return this;
        }

        private AnimationNodeAction _GodotSignalRemovedFromGraph() {
            ExecuteSignal(_onRemovedFromGraphAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private AnimationNodeAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeAction OnTreeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action, oneShot, deferred);
            return this;
        }

        public AnimationNodeAction RemoveOnTreeChanged(Action action) {
            RemoveSignal(_onTreeChangedAction, "tree_changed", nameof(_GodotSignalTreeChanged), action);
            return this;
        }

        private AnimationNodeAction _GodotSignalTreeChanged() {
            ExecuteSignal(_onTreeChangedAction);
            return this;
        }
    }
}