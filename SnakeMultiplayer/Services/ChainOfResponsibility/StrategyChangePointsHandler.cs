using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public class StrategyChangePointsHandler : PointHandler
    {
        public override int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType)
        {
            if (cellType == Cells.strategyChange)
            {
                return arena.AddScore(snake.Key, 2);
            }

            return base.Handle(arena, snake, cellType);
        }
    }
}
