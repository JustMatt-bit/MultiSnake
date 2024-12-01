// File: SnakeMultiplayer/Services/Iterators/IIterator.cs
namespace SnakeMultiplayer.Services.Iterators
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }
}