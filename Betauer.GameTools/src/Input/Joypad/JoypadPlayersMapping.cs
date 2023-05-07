using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Signal;

namespace Betauer.Input.Joypad;

public class JoypadPlayersMapping {
    public int Players => Mapping.Count;
    public readonly List<PlayerMapping> Mapping = new();
    
    private Action? _disconnectGodotSignal;

    public event Action<PlayerMapping> OnPlayerMappingConnectionChanged;

    public JoypadPlayersMapping() {
        _disconnectGodotSignal = SignalExtensions.OnInputJoyConnectionChanged((deviceId, connected) => {
            for (var i = 0; i < Players; i++) {
                var playerMapping = Mapping[i];
                if (playerMapping.JoypadId == deviceId) {
                    playerMapping.SetConnected(connected);
                    OnPlayerMappingConnectionChanged?.Invoke(playerMapping);
                }
            }
        });
    }

    public void Destroy() {
        _disconnectGodotSignal?.Invoke();
        _disconnectGodotSignal = null;
    }

    public PlayerMapping AddPlayer() {
        var mapping = new PlayerMapping(Players);
        Mapping.Add(mapping);
        return mapping;
    }

    public PlayerMapping? GetPlayerMapping(int player) {
        return Mapping[player];
    }

    public List<PlayerMapping> GetPlayersWithJoyPad(int joypadId) {
        return Mapping.Where(mapping => mapping.JoypadId == joypadId).ToList();
    }

    public bool IsJoypadUsed(int joypadId) {
        return Mapping.Find(mapping => mapping.JoypadId == joypadId) != null;
    }

    public int[] GetAllJoypads() {
        return Godot.Input.GetConnectedJoypads().ToArray();
    }

    public int[] GetJoypadIdsAvailable() {
        if (Players == 0) return GetAllJoypads();
        var available = Godot.Input.GetConnectedJoypads().ToList();
        for (var i = 0; i < Players; i++) {
            available.Remove(Mapping[i].JoypadId);
        }
        return available.ToArray();
    }

    public int[] GetJoypadIdsInUse() {
        if (Players == 0) return Array.Empty<int>();
        var joypads = new int[Players];
        for (var i = 0; i < Players; i++) {
            joypads[i] = Mapping[i].JoypadId;
        }
        return joypads;
    }

    public void RemoveAllPlayers() {
        Mapping.Clear();
    }
}