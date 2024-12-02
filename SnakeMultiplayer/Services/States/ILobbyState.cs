using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services;
public interface ILobbyState
{
    LobbyStates getStateName();
    string AddPlayer(LobbyService lobby, string playerName);
    ArenaStatus InitiateGameStart(LobbyService lobby);
    void EndGame(LobbyService lobby);
    ArenaStatus UpdateLobbyState(LobbyService lobby);
}