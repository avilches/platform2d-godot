using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class AnimationNodeStateMachineTransitionAction : AnimationNodeStateMachineTransition {


        private List<Action>? _onAdvanceConditionChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnAdvanceConditionChanged(Action action) {
            if (_onAdvanceConditionChangedAction == null || _onAdvanceConditionChangedAction.Count == 0) {
                _onAdvanceConditionChangedAction ??= new List<Action>(); 
                Connect("advance_condition_changed", this, nameof(_GodotSignalAdvanceConditionChanged));
            }
            _onAdvanceConditionChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnAdvanceConditionChanged(Action action) {
            if (_onAdvanceConditionChangedAction == null || _onAdvanceConditionChangedAction.Count == 0) return this;
            _onAdvanceConditionChangedAction.Remove(action); 
            if (_onAdvanceConditionChangedAction.Count == 0) {
                Disconnect("advance_condition_changed", this, nameof(_GodotSignalAdvanceConditionChanged));
            }
            return this;
        }
        private void _GodotSignalAdvanceConditionChanged() {
            if (_onAdvanceConditionChangedAction == null || _onAdvanceConditionChangedAction.Count == 0) return;
            for (var i = 0; i < _onAdvanceConditionChangedAction.Count; i++) _onAdvanceConditionChangedAction[i].Invoke();
        }
        

        private List<Action>? _onChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnChanged(Action action) {
            if (_onChangedAction == null || _onChangedAction.Count == 0) {
                _onChangedAction ??= new List<Action>(); 
                Connect("changed", this, nameof(_GodotSignalChanged));
            }
            _onChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnChanged(Action action) {
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
        

        private List<Action>? _onScriptChangedAction; 
        public AnimationNodeStateMachineTransitionAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public AnimationNodeStateMachineTransitionAction RemoveOnScriptChanged(Action action) {
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
        
    }
}