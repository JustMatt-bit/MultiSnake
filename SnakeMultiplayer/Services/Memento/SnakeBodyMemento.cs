using System;
using System.Collections.Generic;
using System.Linq;
using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services.Memento
{
    public class SnakeBodyMemento : ISnakeMemento
    {
        private readonly LinkedList<Coordinate> _bodyState;
        private readonly Coordinate _tailCoordinates;
        private readonly IMovementStrategy _movementStrategy;

        public SnakeBodyMemento(LinkedList<Coordinate> bodyState, Coordinate tailCoordinates, IMovementStrategy movementStrategy)
        {
            // Clone the state to ensure immutability of the memento
            _bodyState = new LinkedList<Coordinate>(bodyState.Select(coord => coord.Clone()));
            _tailCoordinates = tailCoordinates;
            _movementStrategy = movementStrategy;
        }

        public SnakeBodyState GetBodyState()
        {
            return new SnakeBodyState()
            {
                BodyCoordinates = new LinkedList<Coordinate>(_bodyState.Select(coord => coord.Clone())),
                TailCoordinates = _tailCoordinates.Clone(),
                CurrentSnakeMovementStrategy = _movementStrategy
            };
        }
    }

    public class SnakeBodyState
    {
        public LinkedList<Coordinate> BodyCoordinates { get; set; }
        public Coordinate TailCoordinates { get; set; }
        public IMovementStrategy CurrentSnakeMovementStrategy { get; set; }
    }
}
