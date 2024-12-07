using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public class ObstaclePointsHandler : PointHandler
    {
        public override int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType)
        {
            if (cellType == Cells.obstacle)
            {
                return arena.DeductScore(snake.Key);
            }

            return base.Handle(arena, snake, cellType);
        }
    }
}
