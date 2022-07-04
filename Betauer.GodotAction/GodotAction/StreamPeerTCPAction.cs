using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class StreamPeerTCPAction : ProxyNode {

        private List<Action>? _onScriptChangedAction; 
        public StreamPeerTCPAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public StreamPeerTCPAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private StreamPeerTCPAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}