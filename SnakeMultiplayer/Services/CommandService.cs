using System;

using SnakeMultiplayer.Services;
using SnakeMultiplayer.Services.Commands;

namespace SnakeMultiplayer.Services
{
    public interface ICommandService
    {
        void HandleCommand(LobbyHub lobbyHub, string lobbyName);
        void UndoCommand(LobbyHub lobbyHub);
    }

    public class CommandService : ICommandService
    {
        private readonly CommandInvoker _commandInvoker;
        private LobbyHub _lobbyhub;
        public CommandService()
        {
            _commandInvoker = new CommandInvoker();
            InitializeCommands();
        }
        
        private void InitializeCommands()
        {
            _commandInvoker.SetCommand("up", new MoveUpCommand(this));
            _commandInvoker.SetCommand("down", new MoveDownCommand(this));
            _commandInvoker.SetCommand("left", new MoveLeftCommand(this));
            _commandInvoker.SetCommand("right", new MoveRightCommand(this));
        }

        public void HandleCommand(LobbyHub lobbyHub, string command)
        {
            _lobbyhub = lobbyHub;
            _commandInvoker.ExecuteCommand(command);
        }

        public void UndoCommand(LobbyHub lobbyHub)
        {
            _lobbyhub = lobbyHub;
            _commandInvoker.UndoLastCommand();
        }
        
        public void UndoLastCommand(MoveDirection direction)
        {
            _lobbyhub.UpdatePlayerState(direction);
        }

        public void SendMovementUpdate(MoveDirection direction)
        {
            _lobbyhub.UpdatePlayerState(direction);
        }        
    }
}