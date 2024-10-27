// File: SnakeMultiplayer/Services/IArenaFactory.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using Microsoft.AspNetCore.HttpOverrides;

using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services
{
    public interface IArenaFactory
    {
        Arena CreateArena(ConcurrentDictionary<string, Snake> players);
        void CreateObstacles(Arena arena);
        Snake CreateSnake(ConcurrentDictionary<string, Snake> currentSnakes, string playerName, IMovementStrategy movementStrategy);
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

        public void CreateObstacles(Arena arena)
        {
            Random random = new Random();
            for (int i = 0; i < 1; i++)
            {
                var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
                arena.AddObstacle(newObstacle);
            }
        }

        public Snake CreateSnake(ConcurrentDictionary<string, Snake> players, string playerName, IMovementStrategy movementStrategy)
        {
            var color = GetValidPlayerColor(players);
            var snake = new Snake(color, false, movementStrategy);

            return snake;
        }

        private PlayerColor GetValidPlayerColor(ConcurrentDictionary<string, Snake> p)
        {
            var players = p.Values.ToList();
            var takenColors = players.Select(p => p.color).ToList();
            var allColors = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();

            foreach (var color in allColors)
            {
                if (!takenColors.Contains(color))
                {
                    return color;
                }
            }

            throw new InvalidOperationException("Cannot find unused player color, because all are used.");
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

        public void CreateObstacles(Arena arena)
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
                arena.AddObstacle(newObstacle);
            }        
        }
        
        public Snake CreateSnake(ConcurrentDictionary<string, Snake> players, string playerName, IMovementStrategy movementStrategy)
        {
            var color = GetValidPlayerColor(players);
            var snake = new Snake(color, false, movementStrategy);

            return snake;
        }

        private PlayerColor GetValidPlayerColor(ConcurrentDictionary<string, Snake> p)
        {
            var players = p.Values.ToList();
            var takenColors = players.Select(p => p.color).ToList();
            var allColors = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();

            foreach (var color in allColors)
            {
                if (!takenColors.Contains(color))
                {
                    return color;
                }
            }

            throw new InvalidOperationException("Cannot find unused player color, because all are used.");
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

        public void CreateObstacles(Arena arena)
        {
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var newObstacle = new Coordinate(random.Next(0, arena.Width), random.Next(0, arena.Height));
                arena.AddObstacle(newObstacle);
            }
        }

        public Snake CreateSnake(ConcurrentDictionary<string, Snake> players, string playerName, IMovementStrategy movementStrategy)
        {
            var color = GetValidPlayerColor(players);
            var snake = new Snake(color, true, movementStrategy);

            return snake;
        }

        private PlayerColor GetValidPlayerColor(ConcurrentDictionary<string, Snake> p)
        {
            var players = p.Values.ToList();
            var takenColors = players.Select(p => p.color).ToList();
            var allColors = Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>().ToList();

            foreach (var color in allColors)
            {
                if (!takenColors.Contains(color))
                {
                    return color;
                }
            }

            throw new InvalidOperationException("Cannot find unused player color, because all are used.");
        }
    }
}