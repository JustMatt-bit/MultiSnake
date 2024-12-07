using System.Collections.Generic;

namespace SnakeMultiplayer.Services.ChainOfResponsibility
{
    public interface IPointHandler
    {
        int Handle(Arena arena, KeyValuePair<string, Snake> snake, Cells cellType);
        IPointHandler SetNext(IPointHandler nextHandler);
    }
}
