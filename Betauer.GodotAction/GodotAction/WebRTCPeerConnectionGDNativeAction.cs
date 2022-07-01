using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction {
    public class WebRTCPeerConnectionGDNativeAction : Node {
        public WebRTCPeerConnectionGDNativeAction() {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }


        private List<Action<Object>>? _onDataChannelReceivedAction; 
        public WebRTCPeerConnectionGDNativeAction OnDataChannelReceived(Action<Object> action, bool oneShot = false, bool deferred = false) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) {
                _onDataChannelReceivedAction ??= new List<Action<Object>>(); 
                GetParent().Connect("data_channel_received", this, nameof(_GodotSignalDataChannelReceived));
            }
            _onDataChannelReceivedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnDataChannelReceived(Action<Object> action) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) return this;
            _onDataChannelReceivedAction.Remove(action); 
            if (_onDataChannelReceivedAction.Count == 0) {
                GetParent().Disconnect("data_channel_received", this, nameof(_GodotSignalDataChannelReceived));
            }
            return this;
        }
        private void _GodotSignalDataChannelReceived(Object channel) {
            if (_onDataChannelReceivedAction == null || _onDataChannelReceivedAction.Count == 0) return;
            for (var i = 0; i < _onDataChannelReceivedAction.Count; i++) _onDataChannelReceivedAction[i].Invoke(channel);
        }
        

        private List<Action<int, string, string>>? _onIceCandidateCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnIceCandidateCreated(Action<int, string, string> action, bool oneShot = false, bool deferred = false) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) {
                _onIceCandidateCreatedAction ??= new List<Action<int, string, string>>(); 
                GetParent().Connect("ice_candidate_created", this, nameof(_GodotSignalIceCandidateCreated));
            }
            _onIceCandidateCreatedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnIceCandidateCreated(Action<int, string, string> action) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) return this;
            _onIceCandidateCreatedAction.Remove(action); 
            if (_onIceCandidateCreatedAction.Count == 0) {
                GetParent().Disconnect("ice_candidate_created", this, nameof(_GodotSignalIceCandidateCreated));
            }
            return this;
        }
        private void _GodotSignalIceCandidateCreated(int index, string media, string name) {
            if (_onIceCandidateCreatedAction == null || _onIceCandidateCreatedAction.Count == 0) return;
            for (var i = 0; i < _onIceCandidateCreatedAction.Count; i++) _onIceCandidateCreatedAction[i].Invoke(index, media, name);
        }
        

        private List<Action>? _onScriptChangedAction; 
        public WebRTCPeerConnectionGDNativeAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) {
                _onScriptChangedAction ??= new List<Action>(); 
                GetParent().Connect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            _onScriptChangedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnScriptChanged(Action action) {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return this;
            _onScriptChangedAction.Remove(action); 
            if (_onScriptChangedAction.Count == 0) {
                GetParent().Disconnect("script_changed", this, nameof(_GodotSignalScriptChanged));
            }
            return this;
        }
        private void _GodotSignalScriptChanged() {
            if (_onScriptChangedAction == null || _onScriptChangedAction.Count == 0) return;
            for (var i = 0; i < _onScriptChangedAction.Count; i++) _onScriptChangedAction[i].Invoke();
        }
        

        private List<Action<string, string>>? _onSessionDescriptionCreatedAction; 
        public WebRTCPeerConnectionGDNativeAction OnSessionDescriptionCreated(Action<string, string> action, bool oneShot = false, bool deferred = false) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) {
                _onSessionDescriptionCreatedAction ??= new List<Action<string, string>>(); 
                GetParent().Connect("session_description_created", this, nameof(_GodotSignalSessionDescriptionCreated));
            }
            _onSessionDescriptionCreatedAction.Add(action);
            return this;
        }
        public WebRTCPeerConnectionGDNativeAction RemoveOnSessionDescriptionCreated(Action<string, string> action) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) return this;
            _onSessionDescriptionCreatedAction.Remove(action); 
            if (_onSessionDescriptionCreatedAction.Count == 0) {
                GetParent().Disconnect("session_description_created", this, nameof(_GodotSignalSessionDescriptionCreated));
            }
            return this;
        }
        private void _GodotSignalSessionDescriptionCreated(string sdp, string type) {
            if (_onSessionDescriptionCreatedAction == null || _onSessionDescriptionCreatedAction.Count == 0) return;
            for (var i = 0; i < _onSessionDescriptionCreatedAction.Count; i++) _onSessionDescriptionCreatedAction[i].Invoke(sdp, type);
        }
        
    }
}