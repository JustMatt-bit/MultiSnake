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
    public class InGameState : ILobbyState
    {

        public LobbyStates getStateName()
        {
            return LobbyStates.inGame;
        }

        public string AddPlayer(LobbyService lobby, string playerName)
        {
            return "Cannot add players during the game.";
        }

        public ArenaStatus InitiateGameStart(LobbyService lobby)
        {
            throw new InvalidOperationException("Game has not started yet.");
        }

        public void EndGame(LobbyService lobby)
        {
            lobby.SetState(new IdleState());
        }

        public ArenaStatus UpdateLobbyState(LobbyService lobby)
        {
            lobby.Arena.UpdateActions();
            if(lobby.Arena.GetScores().Any(score => score.Value >= 15)){
                lobby.Arena.GenerateReport();
                lobby.NotifyObservers("end", "end");
            }
            if (!lobby.IsGameEnd())
                return lobby.Arena.GenerateReport();

            lobby.SetState(new IdleState());
            return null;
        }

    }
}
