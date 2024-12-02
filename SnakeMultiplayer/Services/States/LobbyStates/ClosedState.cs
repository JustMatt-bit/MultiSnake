using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services
{
    public class ClosedState : ILobbyState
    {

         public LobbyStates getStateName()
        {
            return LobbyStates.closed;
        }

        string ILobbyState.AddPlayer(LobbyService lobby, string playerName) => throw new InvalidOperationException("Lobby is closed. Cannot add players.");
        ArenaStatus ILobbyState.InitiateGameStart(LobbyService lobby) => throw new InvalidOperationException("Lobby is closed. Cannot start a game.");
        void ILobbyState.EndGame(LobbyService lobby) => throw new InvalidOperationException("Lobby is already closed.");
        ArenaStatus ILobbyState.UpdateLobbyState(LobbyService lobby) => throw new InvalidOperationException("Lobby is closed. Cannot update player state.");
    }
}
