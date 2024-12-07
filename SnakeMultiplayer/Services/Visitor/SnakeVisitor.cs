namespace SnakeMultiplayer.Services
{
    public class SnakeVisitor : IVisitor
    {
        public void Visit(Snake snake)
        {
            // Toggle the snake's active state
            snake.ToggleActiveState();
        }
    }
}