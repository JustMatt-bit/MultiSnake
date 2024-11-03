using SnakeMultiplayer.Controllers;

namespace SnakeMultiplayer.Services.Commands
{
    public class MoveRightCommand : ICommand
    {
        private readonly CommandService _commandController;
        private readonly MoveDirection _moveDirection = MoveDirection.Right;

        public MoveRightCommand(CommandService commandController)
        {
            _commandController = commandController;
        }

        public void Execute()
        {
            _commandController.SendMovementUpdate(_moveDirection);
        }

        public void Undo()
        {
            _commandController.UndoLastCommand(_moveDirection);
        }
    }
}