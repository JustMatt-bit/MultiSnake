using System;
using System.Collections.Generic;

namespace SnakeMultiplayer.Services.Strategies.Movement
{
    public class WrapAroundStrategy : IMovementStrategy
    {
        private int width, height;

        public WrapAroundStrategy(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public Tuple<LinkedList<Coordinate>, Coordinate> Move(LinkedList<Coordinate> body, Coordinate tail, MoveDirection moveDirection, bool isFood)
        {
            // Clone the current head of the snake
            var newPosition = body.First.Value.Clone();

            // Update the position based on the current movement direction (Left, Right, Up, Down)
            newPosition.Update(moveDirection);

            // **Wrap around logic** for X and Y coordinates:
            newPosition.X = (newPosition.X + width) % width;  // Ensures it stays within the horizontal bounds
            newPosition.Y = (newPosition.Y + height) % height;  // Ensures it stays within the vertical bounds

            // Add the new head position at the front of the snake's body
            body.AddFirst(newPosition);
            tail = null;

            // If the snake didn't eat food, we need to remove the last segment of the snake (to simulate movement)
            if (!isFood)
            {
                tail = body.Last.Value.Clone();  // Capture the position of the tail
                body.RemoveLast();  // Remove the tail from the body
            }

            // Return the updated body and the tail position (if the tail was removed)
            return new Tuple<LinkedList<Coordinate>, Coordinate>(body, tail);
        }
    }
}
