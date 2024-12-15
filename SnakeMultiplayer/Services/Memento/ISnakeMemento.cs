using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Memento
{
    public interface ISnakeMemento
    {
        SnakeBodyState GetBodyState();
    }
}
