using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Input.Joypad;

public class JoypadPlayersMapping {
    public int Players => Mapping.Count;
    public readonly List<PlayerMapping> Mapping = new();
    
    public event Action<PlayerMapping> OnPlayerMappingConnectionChanged;

    public JoypadPlayersMapping() {
        Godot.Input.Singleton.JoyConnectionChanged += JoyConnectionChanged;
    }

    private void JoyConnectionChanged(long deviceId, bool connected) {
        for (var i = 0; i < Players; i++) {
            var playerMapping = Mapping[i];
            if (playerMapping.JoypadId == deviceId) {
                playerMapping.SetConnected(connected);
                OnPlayerMappingConnectionChanged?.Invoke(playerMapping);
            }
        }
    }

    public PlayerMapping AddPlayer() {
        var mapping = new PlayerMapping(this, Players);
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

    public int[] GetJoypadIdsConnected() {
        return Godot.Input.GetConnectedJoypads().ToArray();
    }

    public int[] GetJoypadIdsAvailable() {
        if (Players == 0) return GetJoypadIdsConnected();
        var available = Godot.Input.GetConnectedJoypads().ToList();
        for (var i = 0; i < Players; i++) {
            available.Remove(Mapping[i].JoypadId);
        }
        return available.ToArray();
    }

    public int GetNextJoypadIdAvailable() {
        var available = GetJoypadIdsAvailable();
        return available.Length > 0 ? available[0] : -1;
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