using System;
using System.Collections.Concurrent;

using SnakeMultiplayer.Services.Composite;
using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services
{
    public abstract class ArenaBuilder : IArenaBuilder
    {
        protected readonly IArenaFactory _factory;
        protected Arena _arena;

        public ArenaBuilder(IArenaFactory factory)
        {
            _factory = factory;
        }

        public IArenaBuilder Start(ConcurrentDictionary<string, Snake> players)
        {
            _arena = _factory.CreateArena(players);
            return this;
        }

        public abstract IArenaBuilder SetSpeed(Speed speed);
        public IArenaBuilder SetBoardSize(int width, int height)
        {
            _arena.CreateBoard(width, height);
            return this;
        }

        public abstract IArenaBuilder AddObstacles(IObstacleComponent obstacles);

        public Arena Build()
        {
            return _arena;
        }

        // Template method
        public Arena ConstructArena(ConcurrentDictionary<string, Snake> players, int width, int height, IObstacleComponent obstacles, Speed speed)
        {
            Start(players);
            SetSpeed(speed);
            SetBoardSize(width, height);
            AddObstacles(obstacles);
            return Build();
        }
    }


    public class RandomArenaBuilder : ArenaBuilder
    {
        public RandomArenaBuilder(IArenaFactory factory) : base(factory) { }

        public override IArenaBuilder SetSpeed(Speed speed)
        {
            Random random = new Random();
            int speedCount = Enum.GetValues(typeof(Speed)).Length;
            Speed randomSpeed = (Speed)random.Next(1, speedCount - 1);
            _arena.Speed = randomSpeed;
            return this;
        }

        public override IArenaBuilder AddObstacles(IObstacleComponent obstacles)
        {
            _arena.AddObstacles(obstacles);
            return this;
        }
    }

    public class StandardArenaBuilder : ArenaBuilder
    {
        public StandardArenaBuilder(IArenaFactory factory) : base(factory) { }

        public override IArenaBuilder SetSpeed(Speed speed)
        {
            _arena.Speed = speed;
            return this;
        }

        public override IArenaBuilder AddObstacles(IObstacleComponent obstacles)
        {
            _arena.AddObstacles(obstacles);
            return this;
        }
    }
}