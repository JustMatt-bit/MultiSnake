using System;
using System.Collections.Concurrent;
using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services
{
    public class ArenaBuilder : IArenaBuilder
    {
        private readonly IArenaFactory _factory;
        private Arena _arena;

        public ArenaBuilder(IArenaFactory factory)
        {
            _factory = factory;
        }

        public IArenaBuilder Start(ConcurrentDictionary<string, Snake> players)
        {
            _arena = _factory.CreateArena(players);
            return this;
        }

        public IArenaBuilder SetSpeed(Speed speed)
        {
            _arena.Speed = speed;
            return this;
        }

        public IArenaBuilder SetBoardSize(int width, int height)
        {
            _arena.CreateBoard(width, height);
            return this;
        }

        public IArenaBuilder AddObstacles(int count)
        {
            _factory.CreateObstacles(_arena, count);
            return this;
        }

        public Arena Build()
        {

            return _arena;
        }
    }


    public class RandomArenaBuilder : IArenaBuilder
    {
        private readonly IArenaFactory _factory;
        private Arena _arena;

        public RandomArenaBuilder(IArenaFactory factory)
        {
            _factory = factory;
        }

        public IArenaBuilder Start(ConcurrentDictionary<string, Snake> players)
        {
            _arena = _factory.CreateArena(players);
            return this;
        }

        public IArenaBuilder SetSpeed(Speed speed)
        {

             Random random = new Random();

            int speedCount = Enum.GetValues(typeof(Speed)).Length;

            Speed randomSpeed = (Speed)random.Next(1, speedCount-1);
            _arena.Speed = randomSpeed;
            return this;
        }

        public IArenaBuilder SetBoardSize(int width, int height)
        {

            _arena.CreateBoard(width, height);
            return this;
        }

        public IArenaBuilder AddObstacles(int count)
        {
            Random random = new Random();
    
            int counRand = random.Next(5, 100);
            
            _factory.CreateObstacles(_arena, counRand);
            return this;
        }

        public Arena Build()
        {
            return _arena;
        }
    }
}