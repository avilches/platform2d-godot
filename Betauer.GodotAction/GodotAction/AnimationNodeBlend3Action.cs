using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeBlend3Action : AnimationNodeBlend3 {


        private List<Action>? _onChangedAction; 
        public AnimationNodeBlend3Action OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return this;
            _onChangedAction.Remove(action); 
            if (_onChangedAction.Count == 0) {
                Disconnect("changed", this, nameof(_GodotSignalChanged));
            }
            return this;
        }
        private void _GodotSignalChanged() {
            if (_onChangedAction == null || _onChangedAction.Count == 0) return;
            for (var i = 0; i < _onChangedAction.Count; i++) _onChangedAction[i].Invoke();
        }
        

        private List<Action>? _onRemovedFromGraphAction; 
        public AnimationNodeBlend3Action OnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) {
                _onRemovedFromGraphAction ??= new List<Action>(); 
                Connect("removed_from_graph", this, nameof(_GodotSignalRemovedFromGraph));
            }
            _onRemovedFromGraphAction.Add(action);
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return this;
            _onRemovedFromGraphAction.Remove(action); 
            if (_onRemovedFromGraphAction.Count == 0) {
                Disconnect("removed_from_graph", this, nameof(_GodotSignalRemovedFromGraph));
            }
            return this;
        }
        private void _GodotSignalRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null || _onRemovedFromGraphAction.Count == 0) return;
            for (var i = 0; i < _onRemovedFromGraphAction.Count; i++) _onRemovedFromGraphAction[i].Invoke();
        }
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeBlend3Action OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action>? _onTreeChangedAction; 
        public AnimationNodeBlend3Action OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) {
                _onTreeChangedAction ??= new List<Action>(); 
                Connect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            _onTreeChangedAction.Add(action);
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnTreeChanged(Action action) {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return this;
            _onTreeChangedAction.Remove(action); 
            if (_onTreeChangedAction.Count == 0) {
                Disconnect("tree_changed", this, nameof(_GodotSignalTreeChanged));
            }
            return this;
        }
        private void _GodotSignalTreeChanged() {
            if (_onTreeChangedAction == null || _onTreeChangedAction.Count == 0) return;
            for (var i = 0; i < _onTreeChangedAction.Count; i++) _onTreeChangedAction[i].Invoke();
        }
        
    }
}