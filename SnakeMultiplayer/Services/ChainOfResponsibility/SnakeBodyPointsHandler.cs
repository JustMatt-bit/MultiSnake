using System;
using System.Collections.Generic;

using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public class SnakePointsHandler : PointHandler
    {
        public override int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType)
        {
            if (cellType == Cells.snake)
            {
                if (snake.Value.GetMovementStrategy() is InverseMovementStrategy || snake.Value.GetMovementStrategy() is ZigZagMovementStrategy)
                {
                    return arena.GetCurrentScore(snake.Key);
                }
                else
                {
                    return arena.ResetScore(snake.Key);
                }
            }

            return base.Handle(arena, snake, cellType);
        }
    }
}
