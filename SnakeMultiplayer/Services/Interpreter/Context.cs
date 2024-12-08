namespace SnakeMultiplayer.Services.Interpreter
{
    public class Context
    {
        public CommandService CommandService { get; }
        public LobbyHub LobbyHub { get; }

        public Context(CommandService commandService, LobbyHub lobbyHub)
        {
            CommandService = commandService;
            LobbyHub = lobbyHub;
        }
    }
}