using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.DI.Attributes;

namespace Betauer.Input.Joypad;

public abstract class MultiPlayerContainer {
    public int Players => Mapping.Count;
    protected readonly List<PlayerActionsContainer> Mapping = new();

    [Inject] protected Container? Container { get; set; }

    protected PlayerActionsContainer? GetPlayerActionsById(int playerId) {
        return Mapping.Find(mapping => mapping.PlayerId == playerId);
    }

    protected PlayerActionsContainer? GetPlayerActionsByJoypadId(int joypadId) {
        return Mapping.Find(mapping => mapping.JoypadId == joypadId);
    }

    public bool IsJoypadInUse(int joypadId) {
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

    public int GetNextPlayerId() {
        var playerId = 0;
        while (Mapping.Find(mapping => mapping.PlayerId == playerId) != null) {
            playerId++;
        }
        return playerId;
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
        Mapping.ForEach(container => container.Stop());
        Mapping.Clear();
    }

    public void RemovePlayerActions(int playerId) {
        var playerActionsContainer = Mapping.Find(mapping => mapping.PlayerId == playerId);
        if (playerActionsContainer == null) return;
        playerActionsContainer.Stop();
        Mapping.Remove(playerActionsContainer);
    }

    protected T AddPlayerActions<T>(int joypadId) where T : PlayerActionsContainer {
        return CreatePlayerActions<T>(GetNextPlayerId(), joypadId);
    }

    protected T CreatePlayerActions<T>(int playerId, int joypadId) where T : PlayerActionsContainer {
        if (Mapping.Find(mapping => mapping.PlayerId == playerId) != null) {
            throw new Exception($"Player {playerId} already exists");
        }
        var otherWithSameJoypad = Mapping.Find(mapping => mapping.JoypadId == joypadId);
        if (otherWithSameJoypad != null) {
            throw new Exception($"Player {otherWithSameJoypad.PlayerId} already have the joypad {joypadId}");
        }
        T playerActionsContainer = Activator.CreateInstance<T>();
        Container?.InjectServices(playerActionsContainer);
        playerActionsContainer.LoadFromInstance(playerActionsContainer);
        Mapping.Add(playerActionsContainer);
        playerActionsContainer.Start(playerId, joypadId);
        return playerActionsContainer;
    }
}

public class MultiPlayerContainer<T> : MultiPlayerContainer where T : PlayerActionsContainer {

    public T CreatePlayerActions(int playerId, int joypadId) {
        return CreatePlayerActions<T>(playerId, joypadId);
    }

    public T AddPlayerActions(int joypadId) {
        return AddPlayerActions<T>(joypadId);
    }

    public T? GetPlayerActionsById(int playerId) {
        return base.GetPlayerActionsById(playerId) as T;
    }

    public T? GetPlayerActionsByJoypadId(int joypadId) {
        return base.GetPlayerActionsByJoypadId(joypadId) as T;
    }
}