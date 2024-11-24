using SnakeMultiplayer.Services;
using SnakeMultiplayer.Services.Flyweight;

namespace SnakeMultiplayer.Models
{
    public class Obstacle : IObstacleFlyweight
    {
        public Coordinate Position { get; private set; }
        public string Color { get; private set; }

        public Obstacle(string color = "Red")
        {
            Color = color;
        }

        public void SetPosition(Coordinate extrinsicState) => Position = extrinsicState;
    }
}
