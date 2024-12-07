using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public class FoodPointsHandler : PointHandler
    {
        public override int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType)
        {
            if (cellType == Cells.food)
            {
                return arena.AddScore(snake.Key, 1);
            }

            return base.Handle(arena, snake, cellType);
        }
    }
}
