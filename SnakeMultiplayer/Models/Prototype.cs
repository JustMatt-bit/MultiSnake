namespace SnakeMultiplayer.Models
{
    public interface IPrototype<T>
    {
        T Clone();
    }
}