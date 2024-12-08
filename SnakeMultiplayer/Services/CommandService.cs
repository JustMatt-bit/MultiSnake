using System;

using SnakeMultiplayer.Services;
using SnakeMultiplayer.Services.Commands;
using SnakeMultiplayer.Services.Interpreter;

namespace SnakeMultiplayer.Services
{
public interface ICommandService
{
    void HandleCommand(LobbyHub lobbyHub, string command);
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
        var context = new Context(this, lobbyHub);
        var expression = CommandInterpreter.ParseCommand(command);
        expression.Interpret(command, context);
    }

    public void ExecuteCommand(LobbyHub lobbyHub, string command)
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

    private IExpression ParseCommand(string command)
    {
        var commandParts = command.Split(' ');
        var rootCommand = commandParts[0].ToLower();
        return rootCommand switch
        {
            "move" => new MoveExpression(),
            "undo" => new UndoExpression(),
            _ => null,
        };
    }
}
}