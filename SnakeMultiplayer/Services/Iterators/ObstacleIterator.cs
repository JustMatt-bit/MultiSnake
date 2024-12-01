using System.Collections.Generic;

using SnakeMultiplayer.Services.Composite;

namespace SnakeMultiplayer.Services.Iterators
{
    public class ObstacleIterator : IIterator<IObstacleComponent>
    {
        private readonly List<IObstacleComponent> _components;
        private int _position;

        public ObstacleIterator(List<IObstacleComponent> components)
        {
            _components = components;
            _position = 0;
        }

        public bool HasNext()
        {
            return _position < _components.Count;
        }

        public IObstacleComponent Next()
        {
            return _components[_position++];
        }
    }
}