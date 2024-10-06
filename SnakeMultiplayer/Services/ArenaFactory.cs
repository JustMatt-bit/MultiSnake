// File: SnakeMultiplayer/Services/IArenaFactory.cs
using System.Collections.Concurrent;

using JsonLibrary.FromServer;

namespace SnakeMultiplayer.Services
{
    public interface IArenaFactory
    {
        Arena CreateArena(ConcurrentDictionary<string, Snake> players);
    }

    public class Level1ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            int obstacleCount = 2;
            Speed speed = Speed.Slow;
            return new Arena(players, obstacleCount, speed);
        }
    }

    public class Level2ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            int obstacleCount = 5;
            Speed speed = Speed.Normal;
            return new Arena(players, obstacleCount, speed);
        }
    }

    public class Level3ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            int obstacleCount = 10;
            Speed speed = Speed.Fast;
            return new Arena(players, obstacleCount, speed);
        }
    }
}