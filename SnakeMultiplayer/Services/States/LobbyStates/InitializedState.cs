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
    public class InitializedState : ILobbyState
    {
        public LobbyStates getStateName()
        {
            return LobbyStates.Initialized;
        }

        public string AddPlayer(LobbyService lobby, string playerName)
        {
            return "Cannot add players during game initialization.";
        }

        public ArenaStatus InitiateGameStart(LobbyService lobby)
        {
            _ = lobby.Arena.PrepareForNewGame();
            lobby.SetState(new InGameState());
            Debug.WriteLine($"Game initialised in {lobby.ID} lobby.");
            return lobby.Arena.GenerateReport();
        }

        public void EndGame(LobbyService lobby)
        {
            throw new InvalidOperationException("Game has not started yet.");
        }

        public ArenaStatus UpdateLobbyState(LobbyService lobby)
        {
            throw new InvalidOperationException("Cannot update lobby state during initialization.");
        }
    }
}

