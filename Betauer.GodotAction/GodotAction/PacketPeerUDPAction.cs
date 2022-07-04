using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class PacketPeerUDPAction : ProxyNode {

        private List<Action>? _onScriptChangedAction; 
        public PacketPeerUDPAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public PacketPeerUDPAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private PacketPeerUDPAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }
    }
}