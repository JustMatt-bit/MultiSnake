using SnakeMultiplayer.Services.Interpreter;

public static class CommandInterpreter
    {
    public static IExpression ParseCommand(string command)
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