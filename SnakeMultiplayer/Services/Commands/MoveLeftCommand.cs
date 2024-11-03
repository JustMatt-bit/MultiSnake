using SnakeMultiplayer.Controllers;

namespace SnakeMultiplayer.Services.Commands
{
    public class MoveLeftCommand : ICommand
    {
        private readonly CommandService _commandController;
        private readonly MoveDirection _moveDirection = MoveDirection.Left;

        public MoveLeftCommand(CommandService commandController)
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