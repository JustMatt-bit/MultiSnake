using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public interface IMovementStrategy
    {
        Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate Tail, MoveDirection moveDirection, bool isFood);
    }
}
