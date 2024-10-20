// File: SnakeMultiplayer/Services/IArenaFactory.cs
using System;
using System.Collections.Concurrent;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using Microsoft.AspNetCore.HttpOverrides;

namespace SnakeMultiplayer.Services
{
    public interface IArenaFactory
    {
        Arena CreateArena(ConcurrentDictionary<string, Snake> players);
        void CreateObstaclesInArena(Arena arena);
    }

    public class Level1ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            Speed speed = Speed.Slow;
            var arena = new Arena(players, speed);
            arena.CreateBoard(20, 20);
            return arena;
        }

        public void CreateObstaclesInArena(Arena arena)
        {
            Random random = new Random();
            var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
        }
    }

    public class Level2ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            Speed speed = Speed.Normal;
            var arena = new Arena(players, speed);
            arena.CreateBoard(20, 20);
            return arena;
        }

        public void CreateObstaclesInArena(Arena arena)
        {
            Random random = new Random();
            var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
            newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
        }
    }

    public class Level3ArenaFactory : IArenaFactory
    {
        public Arena CreateArena(ConcurrentDictionary<string, Snake> players)
        {
            Speed speed = Speed.Fast;
            var arena =  new Arena(players, speed);
            arena.CreateBoard(20, 20);
            return arena;
        }

        public void CreateObstaclesInArena(Arena arena)
        {
            Random random = new Random();
            var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
            newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
            newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
            arena.AddObstacle(newObstacle);
        }
    }
}