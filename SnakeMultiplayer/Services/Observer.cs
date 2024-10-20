namespace SnakeMultiplayer.Services
{
    public interface IObserver
    {
        void Update(string operation, ILobbyService record);
    }
}