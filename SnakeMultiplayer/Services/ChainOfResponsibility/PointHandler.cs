using System.Collections.Generic;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public abstract class PointHandler : IPointHandler
    {
        private IPointHandler _nextHandler;

        public virtual int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType)
        {
            return _nextHandler?.Handle(arena, snake, cellType) ?? arena.GetCurrentScore(snake.Key);
        }

        public IPointHandler SetNext(IPointHandler nextHandler)
        {
            _nextHandler = nextHandler;
            return _nextHandler;
        }
    }
}
