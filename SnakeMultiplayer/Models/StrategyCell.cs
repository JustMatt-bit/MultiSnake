using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Models
{
    public class StrategyCell
    {
        public Coordinate Position { get; private set; }
        public string Color { get; private set; } = "Blue";
        public StrategyCell(Coordinate position)
        {
            Position = position;
        }
    }
}
