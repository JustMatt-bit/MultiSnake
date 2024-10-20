using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public class ZigZagMovementStrategy : IMovementStrategy
    {
        private bool toggle = true;

        public Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate tail, MoveDirection direction, bool isFood)
        {
            var newDirection = toggle ? direction : GetPerpendicularDirection(direction);
            toggle = !toggle;

            var newPosition = body.First.Value.Clone();
            newPosition.Update(newDirection);
            _ = body.AddFirst(newPosition);
            tail = null;

            if (!isFood)
            {
                tail = body.Last.Value.Clone();
                body.RemoveLast();
            }

            return new Tuple<LinkedList<Coordinate>, Coordinate>(body, tail);
        }

        private MoveDirection GetPerpendicularDirection(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up or MoveDirection.Down => MoveDirection.Left,
                MoveDirection.Left or MoveDirection.Right => MoveDirection.Up,
                _ => direction,
            };
        }

        public override string ToString() => "Zig Zag Movement";
    }
}
