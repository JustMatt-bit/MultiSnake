using System.Collections.Generic;

namespace SnakeMultiplayer.Services
{
    public interface ISubject
    {
        void RegisterObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers(string operation, string record);
    }
}