using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class VisualShaderNodeTransformFuncAction : ProxyNode {

        private List<Action>? _onChangedAction; 
        public VisualShaderNodeTransformFuncAction OnChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onChangedAction, "changed", nameof(_GodotSignalChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeTransformFuncAction RemoveOnChanged(Action action) {
            RemoveSignal(_onChangedAction, "changed", nameof(_GodotSignalChanged), action);
            return this;
        }

        private VisualShaderNodeTransformFuncAction _GodotSignalChanged() {
            ExecuteSignal(_onChangedAction);
            return this;
        }

        private List<Action>? _onEditorRefreshRequestAction; 
        public VisualShaderNodeTransformFuncAction OnEditorRefreshRequest(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeTransformFuncAction RemoveOnEditorRefreshRequest(Action action) {
            RemoveSignal(_onEditorRefreshRequestAction, "editor_refresh_request", nameof(_GodotSignalEditorRefreshRequest), action);
            return this;
        }

        private VisualShaderNodeTransformFuncAction _GodotSignalEditorRefreshRequest() {
            ExecuteSignal(_onEditorRefreshRequestAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public VisualShaderNodeTransformFuncAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public VisualShaderNodeTransformFuncAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private VisualShaderNodeTransformFuncAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}