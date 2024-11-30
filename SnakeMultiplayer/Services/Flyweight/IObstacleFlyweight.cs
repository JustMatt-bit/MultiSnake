using System.Drawing;

namespace SnakeMultiplayer.Services.Flyweight
{
    public interface IObstacleFlyweight
    {
        public void PlaceOnBoard(Coordinate position, Cells[,] board);
    }
}
