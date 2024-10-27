using System.Collections.Concurrent;
using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services
{
    public interface IArenaBuilder
    {
        IArenaBuilder Start(ConcurrentDictionary<string, Snake> players);
        IArenaBuilder SetSpeed(Speed speed);
        IArenaBuilder SetBoardSize(int width, int height);
        IArenaBuilder AddObstacles(int count);
        Arena Build();
    }
}