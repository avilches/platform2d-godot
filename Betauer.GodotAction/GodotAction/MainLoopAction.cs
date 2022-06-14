using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class MainLoopAction : MainLoop {


        private Action<bool, string>? _onRequestPermissionsResultAction; 
        public MainLoopAction OnRequestPermissionsResult(Action<bool, string> action) {
            if (_onRequestPermissionsResultAction == null) 
                Connect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            _onRequestPermissionsResultAction = action;
            return this;
        }
        public MainLoopAction RemoveOnRequestPermissionsResult() {
            if (_onRequestPermissionsResultAction == null) return this; 
            Disconnect("on_request_permissions_result", this, nameof(ExecuteRequestPermissionsResult));
            _onRequestPermissionsResultAction = null;
            return this;
        }
        private void ExecuteRequestPermissionsResult(bool granted, string permission) =>
            _onRequestPermissionsResultAction?.Invoke(granted, permission);
        

        private Action? _onScriptChangedAction; 
        public MainLoopAction OnScriptChanged(Action action) {
            if (_onScriptChangedAction == null) 
                Connect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = action;
            return this;
        }
        public MainLoopAction RemoveOnScriptChanged() {
            if (_onScriptChangedAction == null) return this; 
            Disconnect("script_changed", this, nameof(ExecuteScriptChanged));
            _onScriptChangedAction = null;
            return this;
        }
        private void ExecuteScriptChanged() =>
            _onScriptChangedAction?.Invoke();
        
    }
}