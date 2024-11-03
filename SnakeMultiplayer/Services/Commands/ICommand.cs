namespace SnakeMultiplayer.Services.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}