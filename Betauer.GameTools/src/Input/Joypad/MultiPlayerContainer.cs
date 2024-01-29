using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.Input.Joypad;

public class MultiPlayerContainer<T> where T : PlayerActionsContainer {
    
    public static readonly Logger Logger = LoggerFactory.GetLogger<MultiPlayerContainer<T>>();

    public int Players => Mapping.Count;
    public List<T> Mapping { get; } = new();
    public InputActionsContainer SharedInputActionsContainer { get; private set; } = new();
    public T SharedInputActions { get; private set; }

    public bool Running { get; private set; } = false;

    public MultiPlayerContainer() {
        SharedInputActions = Create();
        SharedInputActionsContainer.AddActionsFromProperties(SharedInputActions);
    }

    public void ConfigureSaveSettings(SettingsContainer settingsContainer) {
        SharedInputActionsContainer.ConfigureSaveSettings(settingsContainer);
    }

    public void Start() {
        if (Running) throw new Exception($"Cant' start {GetType().GetTypeName()}: instance is already running.");
        // Create the only instance of RedefineInputPlayerActionsContainer with settings
        Godot.Input.Singleton.JoyConnectionChanged += OnJoyConnectionChanged;
        Running = true;
    }

    public void Stop() {
        if (!Running) return;
        Godot.Input.Singleton.JoyConnectionChanged -= OnJoyConnectionChanged;
        Running = false;
        RemoveAllPlayers();
    }

    /// <summary>
    /// This method must be used when the player redefine any action in the SharedInputActions so the changes are reflected in every T.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void SyncActions() {
        Mapping.ForEach(playerActionsContainer => playerActionsContainer.SyncActions());
    }

    private void OnJoyConnectionChanged(long device, bool connected) {
        Logger.Debug("OnJoyConnectionChanged {0} {1}", device, connected ? "connected" : "disconnected");
        var playerAction = GetPlayerActionsByJoypadId((int)device);
        playerAction?.JoyConnectionChanged(connected);
    }

    public bool IsJoypadInUse(int joypadId) {
        return Mapping.Any(mapping => mapping.JoypadId == joypadId);
    }

    public bool IsKeyboardInUse() {
        return Mapping.Any(mapping => mapping.Keyboard);
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

    public int GetNextPlayerId() {
        var playerId = 0;
        while (Mapping.Any(mapping => mapping.PlayerId == playerId)) {
            playerId++;
        }
        return playerId;
    }

    public T? GetPlayerActionsById(int playerId) {
        return Mapping.Find(mapping => mapping.PlayerId == playerId);
    }

    public T? GetPlayerActionsByJoypadId(int joypadId) {
        return Mapping.Find(mapping => mapping.JoypadId == joypadId);
    }

    public T? GetPlayerActionsByKeyboard() {
        return Mapping.Find(mapping => mapping.Keyboard);
    }

    public T AddPlayerActions(int joypadId, bool keyboard = false) {
        return CreatePlayerActions(GetNextPlayerId(), joypadId, keyboard);
    }

    public void ChangeJoypad(int playerId, int newJoypadId) {
        var playerActions = GetPlayerActionsById(playerId);
        if (playerActions == null) throw new Exception($"Player {playerId} not found");
        playerActions.ChangeJoypad(newJoypadId);
    }

    public void ChangeKeyboard(int playerId, bool newKeyboard) {
        var playerActions = GetPlayerActionsById(playerId);
        if (playerActions == null) throw new Exception($"Player {playerId} not found");
        playerActions.ChangeKeyboard(newKeyboard);
    }

    public void ChangePlayerId(int oldPlayerId, int newPlayerId) {
        if (oldPlayerId == newPlayerId) return;
        var oldPlayerActions = GetPlayerActionsById(oldPlayerId);
        if (oldPlayerActions == null) throw new Exception($"Player {oldPlayerId} not found");

        var newPlayerActions = GetPlayerActionsById(newPlayerId);
        if (newPlayerActions != null) throw new Exception($"Can't change player {oldPlayerId} to {newPlayerId}: there is already a player with {newPlayerId} id");
        oldPlayerActions.ChangePlayerId(newPlayerId);
    }

    /// <summary>
    /// It's your responsibility to ensure that 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="joypadId"></param>
    /// <param name="keyboard"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public T CreatePlayerActions(int playerId, int joypadId, bool keyboard = false) {
        if (GetPlayerActionsById(playerId) != null) {
            throw new Exception($"Player {playerId} already exists");
        }
        var playerActions = Create();
        Logger.Debug("Creating player {0} with joypad {1} {2}", playerId, joypadId, keyboard ? "and keyboard" : "");
        playerActions.Start(playerId, joypadId, keyboard, SharedInputActionsContainer);
        Mapping.Add(playerActions);
        return playerActions;
    }

    public void RemoveAllPlayers() {
        Mapping.ToList().ForEach(RemovePlayerActions); // The ToList clones the list so we can remove items while iterating without problems
    }

    public void RemovePlayerActions(T playerActionsContainer) {
        Logger.Debug($"Stopping player {playerActionsContainer.PlayerId}");
        playerActionsContainer.Stop();
        Mapping.Remove(playerActionsContainer);
    }

    public void RemovePlayerActions(int playerId) {
        var playerActionsContainer = Mapping.Find(mapping => mapping.PlayerId == playerId);
        if (playerActionsContainer == null) return;
        RemovePlayerActions(playerActionsContainer);
    }

    private T Create() {
        T playerActionsContainer = Activator.CreateInstance<T>();
        return playerActionsContainer;
    }
}