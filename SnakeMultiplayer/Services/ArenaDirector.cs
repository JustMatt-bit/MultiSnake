using System.Collections.Concurrent;

namespace SnakeMultiplayer.Services
{
    public class ArenaDirector
    {
        private IArenaBuilder _builder;

        public void SetBuilder(IArenaBuilder builder)
        {
            _builder = builder;
        }

        public Arena ConstructArena(ConcurrentDictionary<string, Snake> players, int width, int height, int obstacleCount, Speed speed)
        {
            return _builder.Start(players)
                           .SetSpeed(speed)
                           .SetBoardSize(width, height)
                           .AddObstacles(obstacleCount)
                           .Build();
        }
    }
}