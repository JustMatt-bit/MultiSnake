using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using SnakeMultiplayer.Services.Strategies.Movement;
namespace SnakeMultiplayer.Services
{
    public class IdleState : ILobbyState
    {

        public LobbyStates getStateName()
        {
            return LobbyStates.Idle;
        }

        public string AddPlayer(LobbyService lobby, string playerName)
        {
            var reason = lobby.CanJoin(playerName);
            if (!string.IsNullOrWhiteSpace(reason))
                return reason;

            var snake = lobby.factory.CreateSnake(lobby.players, playerName, new DefaultMovementStrategy());
           
            if (!lobby.players.TryAdd(playerName, snake))
                return "An error has occurred. Please try again later.";

            return string.Empty;
        }

        public ArenaStatus InitiateGameStart(LobbyService lobby)
        {
            lobby.SetState(new InitializedState());
            
            return lobby.InitiateGameStart();
        }

        public void EndGame(LobbyService lobby)
        {
            throw new InvalidOperationException("Cannot end the game in Idle state.");
        }

        public ArenaStatus UpdateLobbyState(LobbyService lobby)
        {
            throw new InvalidOperationException("No game is active in Idle state.");
        }
    }
}
