using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public class DefaultMovementStrategy : IMovementStrategy
    {
        public Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate Tail, MoveDirection moveDirection, bool isFood)
        {
            var newPosition = body.First.Value.Clone();
            newPosition.Update(moveDirection);
            _ = body.AddFirst(newPosition);
            Tail = null;

            if (!isFood)
            {
                Tail = body.Last.Value.Clone();
                body.RemoveLast();
            }

            return new Tuple<LinkedList<Coordinate>, Coordinate>(body, Tail);
        }

        public override string ToString() => "Default Movement";
    }
}
