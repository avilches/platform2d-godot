using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeBlend3Action : AnimationNodeBlend3 {


        private Action? _onChangedAction; 
        public AnimationNodeBlend3Action OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onRemovedFromGraphAction; 
        public AnimationNodeBlend3Action OnRemovedFromGraph(Action action) {
            if (_onRemovedFromGraphAction == null) 
                Connect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = action;
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnRemovedFromGraph() {
            if (_onRemovedFromGraphAction == null) return this; 
            Disconnect("removed_from_graph", this, nameof(ExecuteRemovedFromGraph));
            _onRemovedFromGraphAction = null;
            return this;
        }
        private void ExecuteRemovedFromGraph() =>
            _onRemovedFromGraphAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public AnimationNodeBlend3Action OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        

        private Action? _onTreeChangedAction; 
        public AnimationNodeBlend3Action OnTreeChanged(Action action) {
            if (_onTreeChangedAction == null) 
                Connect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = action;
            return this;
        }
        public AnimationNodeBlend3Action RemoveOnTreeChanged() {
            if (_onTreeChangedAction == null) return this; 
            Disconnect("tree_changed", this, nameof(ExecuteTreeChanged));
            _onTreeChangedAction = null;
            return this;
        }
        private void ExecuteTreeChanged() =>
            _onTreeChangedAction?.Invoke();
        
    }
}