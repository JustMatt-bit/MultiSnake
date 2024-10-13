using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public class InverseMovementStrategy : IMovementStrategy
    {
        private bool _isFirstMove = true;

        public Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate tail, MoveDirection moveDirection, bool isFood)
        {
            if (_isFirstMove)
            {
                var tempHead = body.First.Value;
                body.RemoveFirst();
                body.AddLast(tempHead);
                _isFirstMove = false;
            }

            var newDirection = InverseDirection(moveDirection);
            var newPosition = body.First.Value.Clone();
            newPosition.Update(newDirection);
            body.AddFirst(newPosition);
            tail = null;

            if (!isFood)
            {
                tail = body.Last.Value.Clone();
                body.RemoveLast();
            }
            else
            {
                Console.WriteLine("maistas");
            }

            return new Tuple<LinkedList<Coordinate>, Coordinate>(body, tail);
        }

        private static MoveDirection InverseDirection (MoveDirection moveDirection)
        {
            return moveDirection switch
            {
                MoveDirection.Left => MoveDirection.Right,
                MoveDirection.Right => MoveDirection.Left,
                MoveDirection.Up => MoveDirection.Down,
                MoveDirection.Down => MoveDirection.Up,
                _ => throw new ArgumentException($"Argument value of enum CoordDirection expected, but {moveDirection} found"),
            };
        }
    }
}
