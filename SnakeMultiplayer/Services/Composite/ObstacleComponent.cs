using System.Collections.Generic;

using SnakeMultiplayer.Services.Iterators;

namespace SnakeMultiplayer.Services.Composite
{
    public interface IObstacleComponent
    {
        void AddToBoard(Arena arena);
        IIterator<IObstacleComponent> CreateIterator();
    }

    public class SingleObstacle : IObstacleComponent
    {
        private readonly Coordinate _position;

        public SingleObstacle(Coordinate position)
        {
            _position = position;
        }

        public void AddToBoard(Arena arena)
        {
            arena.AddObstacle(_position);
        }

        public IIterator<IObstacleComponent> CreateIterator()
        {
            return new ObstacleIterator(new List<IObstacleComponent> { this });
        }
    }

    public class ObstacleGroup : IObstacleComponent
    {
        private readonly List<IObstacleComponent> _components = new();

        public void Add(IObstacleComponent component)
        {
            _components.Add(component);
        }

        public void Remove(IObstacleComponent component)
        {
            _components.Remove(component);
        }

        public void AddToBoard(Arena arena)
        {
            foreach (var component in _components)
            {
                component.AddToBoard(arena);
            }
        }

        public IIterator<IObstacleComponent> CreateIterator()
        {
            return new ObstacleIterator(_components);
        }
    }
}