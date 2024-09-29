﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SnakeMultiplayer.Services;

public interface IGameServerService
{
    string AddPlayerToLobby(string lobby, string player);
    string CanJoin(string lobbyName, string playerName);
    void EndGame(string lobby);
    ILobbyService GetLobbyService(string lobby);
    List<Tuple<string, string>> GetLobbyStatus();
    bool LobbyExists(string lobbyName);
    bool PlayerExists(string lobbyName, string playerName);
    void RemoveLobby(string lobby);
    bool TryCreateLobby(string lobbyName, int obstacleCount, string hostPlayerName, IGameServerService service);
}

/// <summary>
/// Gives abstraction layer to web socket based communication:
/// Distributes incoming messages to relevant lobbies and 
/// forwads messages from lobbies to web sockets
/// </summary>
public class GameServerService : IGameServerService
{
    //TODO: Move to constants
    public static Regex ValidStringRegex = new(@"^[a-zA-Z0-9]+[a-zA-Z0-9\s_]*[a-zA-Z0-9]+$");
    readonly int MaxPlayersInLobby = 4;

    readonly ConcurrentDictionary<string, LobbyService> lobbies = new();

    public string AddPlayerToLobby(string lobby, string player)
    {
        try
        {
            return lobbies[lobby].AddPlayer(player);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool TryCreateLobby(string lobbyName, int obstacleCount, string hostPlayerName, IGameServerService service)
        => lobbies.TryAdd(lobbyName, new LobbyService(lobbyName, obstacleCount, hostPlayerName, MaxPlayersInLobby));

    public string CanJoin(string lobbyName, string playerName) =>
        !lobbies.TryGetValue(lobbyName, out var lobby)
            ? $"Lobby {lobbyName} does not exist. Please try a different name"
        : lobby.CanJoin(playerName);

    public bool LobbyExists(string lobbyName) => lobbies.ContainsKey(lobbyName);

    public bool PlayerExists(string lobbyName, string playerName) =>
        lobbies.TryGetValue(lobbyName, out var lobby)
            ? lobby.PlayerExists(playerName)
        : throw new EntryPointNotFoundException($"Lobby {lobbyName} does not exists");

    public void EndGame(string lobby)
    {
        if (lobbies.TryGetValue(lobby, out var lobbyService))
            lobbyService.EndGame();
    }

    public void RemoveLobby(string lobby)
    {
        if (lobby == null)
            throw new ArgumentNullException(nameof(lobby), "Tried to remove null lobby from lobby dictionary");

        _ = lobbies.TryRemove(lobby, out _);
    }

    public List<Tuple<string, string>> GetLobbyStatus()
    {
        var lobbyList = new List<Tuple<string, string>>(lobbies.Count);

        foreach (var pair in lobbies)
        {
            lobbyList.Add(new Tuple<string, string>(pair.Key, pair.Value.GetPlayerCount().ToString()));
        }
        return lobbyList;
    }

    public ILobbyService GetLobbyService(string lobby) => lobbies[lobby];
}