using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Models
{
    public class Obstacle
    {
        public Coordinate Position { get; private set; }
        public string Color { get; private set; } = "Red"; // Default color

        public Obstacle(Coordinate position)
        {
            Position = position;
        }
    }
}
