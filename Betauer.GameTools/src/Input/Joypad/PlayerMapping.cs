﻿using System;

namespace Betauer.Input.Joypad;

public class PlayerMapping {
    public int Player { get; }
    public int JoypadId { get; private set; } = -1;
    public bool Connected { get; internal set; } = false;

    public event Action OnJoypadDisconnect;
    public event Action OnJoypadConnect;
    public event Action OnJoypadChanged;

    internal PlayerMapping(int player) {
        Player = player;
    }

    public PlayerMapping SetJoypadId(int joypadId) {
        if (joypadId != JoypadId) {
            JoypadId = joypadId;
            Connected = Godot.Input.GetConnectedJoypads().Contains(joypadId);
            OnJoypadChanged?.Invoke();
        }
        return this;
    }

    public override string ToString() {
        return $"P{Player}:{JoypadId}{(Connected ? "" : "(disconnected)")}";
    }

    internal void SetConnected(bool connected) {
        if (Connected == connected) return;
        Connected = connected;
        if (Connected) OnJoypadConnect?.Invoke();
        else OnJoypadDisconnect?.Invoke();
    }
}