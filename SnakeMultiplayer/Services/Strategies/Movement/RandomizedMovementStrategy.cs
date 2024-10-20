using System.Collections.Generic;
using System;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public class RandomMovementStrategy : IMovementStrategy
    {
        private static readonly Random random = new Random();

        public Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate tail, MoveDirection direction, bool isFood)
        {
            var randomDirection = (MoveDirection)random.Next(0, 4);
            var newPosition = body.First.Value.Clone();
            newPosition.Update(randomDirection);
            _ = body.AddFirst(newPosition);
            tail = null;

            if (!isFood)
            {
                tail = body.Last.Value.Clone();
                body.RemoveLast();
            }

            return new Tuple<LinkedList<Coordinate>, Coordinate>(body, tail);
        }

        public override string ToString() => "Randomized movement";
    }
}
