using System.Collections.Generic;

using SnakeMultiplayer.Models;

namespace SnakeMultiplayer.Services.Flyweight
{
    public class ObstacleFlyweightFactory
    {
        private readonly Dictionary<string, IObstacleFlyweight> _obstacleFlyweights = new();
        public IObstacleFlyweight GetFlyWeight(string color)
        {
            if (_obstacleFlyweights.TryGetValue(color, out var boardCell))
            {
                return boardCell;
            }

            return _obstacleFlyweights[color] = new Obstacle(color);
        }
    }
}
