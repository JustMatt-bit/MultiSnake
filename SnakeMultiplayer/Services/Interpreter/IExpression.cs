namespace SnakeMultiplayer.Services.Interpreter
{
    public interface IExpression
    {
        void Interpret(string command, Context context);
    }

    public class MoveExpression : IExpression
    {
        public void Interpret(string command, Context context)
        {
            switch (command)
            {
                case "move up":
                    context.CommandService.ExecuteCommand(context.LobbyHub, "up");
                    break;
                case "move down":
                    context.CommandService.ExecuteCommand(context.LobbyHub, "down");
                    break;
                case "move left":
                    context.CommandService.ExecuteCommand(context.LobbyHub, "left");
                    break;
                case "move right":
                    context.CommandService.ExecuteCommand(context.LobbyHub, "right");
                    break;
                default:
                    break;
            }
        }
    }

    public class UndoExpression : IExpression
    {
        public void Interpret(string command, Context context)
        {
            context.CommandService.UndoCommand(context.LobbyHub);
        }
    }
}