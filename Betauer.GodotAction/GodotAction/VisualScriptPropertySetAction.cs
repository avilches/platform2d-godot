using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualScriptPropertySetAction : VisualScriptPropertySet {


        private Action? _onChangedAction; 
        public VisualScriptPropertySetAction OnChanged(Action action) {
            if (_onChangedAction == null) 
                Connect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = action;
            return this;
        }
        public VisualScriptPropertySetAction RemoveOnChanged() {
            if (_onChangedAction == null) return this; 
            Disconnect("changed", this, nameof(ExecuteChanged));
            _onChangedAction = null;
            return this;
        }
        private void ExecuteChanged() =>
            _onChangedAction?.Invoke();
        

        private Action? _onPortsChangedAction; 
        public VisualScriptPropertySetAction OnPortsChanged(Action action) {
            if (_onPortsChangedAction == null) 
                Connect("ports_changed", this, nameof(ExecutePortsChanged));
            _onPortsChangedAction = action;
            return this;
        }
        public VisualScriptPropertySetAction RemoveOnPortsChanged() {
            if (_onPortsChangedAction == null) return this; 
            Disconnect("ports_changed", this, nameof(ExecutePortsChanged));
            _onPortsChangedAction = null;
            return this;
        }
        private void ExecutePortsChanged() =>
            _onPortsChangedAction?.Invoke();
        

        private Action? _onScriptChangedAction; 
        public VisualScriptPropertySetAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public VisualScriptPropertySetAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}