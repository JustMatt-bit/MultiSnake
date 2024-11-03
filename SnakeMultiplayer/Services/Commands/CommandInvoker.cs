using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Commands
{
    public class CommandInvoker
    {
        private readonly Dictionary<string, ICommand> _commands;
        private readonly Stack<ICommand> _commandHistory;

        public CommandInvoker()
        {
            _commands = new Dictionary<string, ICommand>();
            _commandHistory = new Stack<ICommand>();
        }

        public void SetCommand(string commandName, ICommand command)
        {
            _commands[commandName] = command;
        }

        public void ExecuteCommand(string commandName)
        {
            if (_commands.TryGetValue(commandName, out var command))
            {
                _commandHistory.Push(command);
                command.Execute();
            }
            else
            {
                Console.WriteLine($"Command {commandName} not found.");
            }
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count <= 1)
            {
                Console.WriteLine("No commands to undo.");
                return;
            }

            var lastCommand = _commandHistory.Pop();
            var commandToUndo = _commandHistory.Pop();
            commandToUndo.Undo();
        }
    }
}