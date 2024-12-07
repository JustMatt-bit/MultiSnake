using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonLibrary.FromClient;
using JsonLibrary.FromServer;
using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services.Strategies.Movement;
using SnakeMultiplayer.Services.Appearance;
using SnakeMultiplayer.Services.Composite;
using SnakeMultiplayer.Services.Flyweight;
using SnakeMultiplayer.Services.ChainOfResponsibility;

namespace SnakeMultiplayer.Services;

public class Arena
{
    public Speed Speed { get;  set; }
    public IPointHandler PointHandlerChain { get; private set; }

    readonly Random Random = new(Guid.NewGuid().GetHashCode());
    readonly ConcurrentDictionary<string, Snake> Snakes;
    private Dictionary<string, Snake> clonedSnakes = new Dictionary<string, Snake>();
    private readonly ObstacleFlyweightFactory _obstacleFactory = new();

    readonly ConcurrentDictionary<string, MoveDirection> PendingActions;
    readonly int _strategiesCount = 4;

    ConcurrentDictionary<string, int> Scores;
    ConcurrentDictionary<string, MoveDirection> PreviousActions;
    Cells[,] Board;
    public int Width;
    public int Height;
    bool IsWall;
    Coordinate Food;
    LinkedList<(IObstacleFlyweight Obstacle, Coordinate Position)> Obstacles;
    StrategyCell StrategyCell;

    public Arena(ConcurrentDictionary<string, Snake> players, Speed speed)
    {
        Snakes = players;
        PendingActions = new ConcurrentDictionary<string, MoveDirection>();
        Scores = new ConcurrentDictionary<string, int>();
        Food = null;
        Obstacles = new LinkedList<(IObstacleFlyweight Obstacle, Coordinate Position)>();
        Speed = speed;
        InitializePointHandlers();
    }


    public void CreateBoard(int width, int height) {
        Width = width;
        Height = height;
        Board = new Cells[Width, Height];
    }

    public ArenaStatus GenerateReport()
    {
        obstacleXY[] obstaclesArray = Obstacles.Select(o => new obstacleXY(new XY(o.Position.X, o.Position.Y), ((Obstacle)o.Obstacle).Color)).ToArray();

        // Create an ArenaStatus instance with food and obstacles
        var report = new ArenaStatus(
            Food == null ? null : new XY(Food.X, Food.Y),
            obstaclesArray,
            strategyCell: new StrategyCellXY(new XY(StrategyCell.Position.X, StrategyCell.Position.Y), StrategyCell.Color)
        );

        foreach (var snake in Snakes)
        {
            if (snake.Value == null)
            {
                continue;
            }
            else if (!snake.Value.IsActive)
            {
                report.AddDisabledSnake(snake.Key);
            }
            else if (snake.Value.IsRevived) {
                var head = snake.Value.CloneHead().ConvertToXY();
                var tail = snake.Value.Tail?.ConvertToXY();
                var color = snake.Value.GetColorString();
                var shape = snake.Value.shape;
                var score = GetScore(snake.Key);
                var isStriped = snake.Value.IsStriped;
                var appearance = snake.Value.Appearance;
                var body = snake.Value.GetBodyAsCoordinateList().Select(coord => coord.ConvertToXY()).ToList(); // Convert to List<XY>
                var currentMovingStrategy = snake.Value.GetMovementStrategy().ToString();
                var crownStage = snake.Value.CrownStage.ToString();
                var tempSnake = new JsonLibrary.FromServer.Snake(snake.Key, color, currentMovingStrategy, head, tail, body, score, isStriped, appearance.ShapeName, crownStage);
                report.AddSnakeToRevive(tempSnake);
                snake.Value.IsRevived = false;
            }
            else
            {
                var head = snake.Value.CloneHead().ConvertToXY();
                var tail = snake.Value.Tail?.ConvertToXY();
                var color = snake.Value.GetColorString();
                var score = GetScore(snake.Key);
                var isStriped = snake.Value.IsStriped;
                var appearance = snake.Value.Appearance;
                var currentMovingStrategy = snake.Value.GetMovementStrategy().ToString();
                var crownStage = snake.Value.CrownStage.ToString();
                var tempSnake = new JsonLibrary.FromServer.Snake(snake.Key, color, currentMovingStrategy, head, tail, null, score, isStriped, appearance.ShapeName, crownStage);
                report.AddActiveSnake(tempSnake);
            }
        }
        return report;
    }

    public async void ReviveSnake(string playerName)
    {
        if (clonedSnakes.TryGetValue(playerName, out var clonedSnake))
        {
            await Task.Delay(3000);
            Snakes[playerName] = clonedSnake;
            clonedSnakes.Remove(playerName);
            Snakes[playerName].Activate();
            Snakes[playerName].IsRevived = true;
        }
    }


    // Inefficient. 
    // TODO: get random coordinates and use breath-first search algorithm 
    // to find nearest empty cell.
    /// <summary>
    /// If needed, generates food at random location in the board.
    /// </summary>
    /// <param name="force">If true, generates food even if its nulll</param>
    public void GenerateFood(bool force)
    {
        if (force || Food != null)
        {
            return;
        }

        Food = null;
        while (true)
        {
            var newFood = new Coordinate(Random.Next(0, Width), Random.Next(0, Height));

            // Check if the newFood position is occupied by a snake or an obstacle
            bool containsSnake = Snakes.Values.Any(snake => snake.Contains(newFood));
            bool isObstacleCell = Obstacles.Any(obstacle => obstacle.Position.X == newFood.X && obstacle.Position.Y == newFood.Y);
            bool isStrategyCell = newFood.X == StrategyCell.Position.X && newFood.Y == StrategyCell.Position.Y;

            // If it's not occupied by a snake or an obstacle, set the food
            if (!containsSnake && !isObstacleCell && !isStrategyCell)
            {
                Food = newFood;
                Board[newFood.X, newFood.Y] = Cells.food;
                return; // Set the flag to exit the loop
            }
        }

        //TODO: Player won? Refactor to better logic.
        throw new Exception("Could not set food");
    }

    // public void GenerateObstacles()
    // {
    //     for (int i = 0; i < Obstacles.Count(); i++)
    //     {
    //         bool isObstacleSet = false;

    //         while (!isObstacleSet)
    //         {
    //             var newObstacle = new Coordinate(Random.Next(0, Width), Random.Next(0, Height));

    //             // Ensure the obstacle isn't on a snake or food
    //             var containsSnake = Snakes.Values.Any(snake => snake.Contains(newObstacle));
    //             var isFoodCell = Board[newObstacle.X, newObstacle.Y] == Cells.food;

    //             if (!containsSnake && !isFoodCell)
    //             {
    //                 Obstacles[i] = new Obstacle(newObstacle); // Set obstacle in the array
    //                 Board[newObstacle.X, newObstacle.Y] = Cells.obstacle; // Mark the board cell as an obstacle
    //                 isObstacleSet = true;
    //             }
    //         }
    //     }
    // }

    public void AddObstacle(Coordinate obstaclePosition) {
        if (obstaclePosition == null)
        {
            throw new ArgumentNullException(nameof(obstaclePosition));
        }

        // Ensure the obstacle position is within the bounds of the arena
        if (obstaclePosition.X < 0 || obstaclePosition.X >= Width || obstaclePosition.Y < 0 || obstaclePosition.Y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(obstaclePosition), "Obstacle position is out of bounds.");
        }

        var obstacleType = _obstacleFactory.GetFlyWeight("red");

        // Set the obstacle position and update the board
        obstacleType.PlaceOnBoard(obstaclePosition, Board);
        Obstacles.AddLast((obstacleType, obstaclePosition));
        //Board[obstaclePosition.X, obstaclePosition.Y] = Cells.obstacle;
    }

    public void AddObstacles(IObstacleComponent obstacles)
    {
        for (var iterator = obstacles.CreateIterator(); iterator.HasNext();)
        {
            var obstacle = iterator.Next();
            obstacle.AddToBoard(this);    
        }
    }

    public void GenerateStrategyCell()
    {
        if (StrategyCell != null)
        {
            return;
        }

        bool isStrategyCellSet = false;
        while (!isStrategyCellSet)
        {
            var newStrategyCell = new Coordinate(Random.Next(0, Width), Random.Next(0, Height));
            var containsSnake = Snakes.Values.Any(snake => snake.Contains(newStrategyCell));
            var isFoodCell = Board[newStrategyCell.X, newStrategyCell.Y] == Cells.food;
            var isObstacleCell = Obstacles.Any(obstacle => obstacle.Position.X == newStrategyCell.X && obstacle.Position.Y == newStrategyCell.Y);

            if (!containsSnake && !isFoodCell && !isObstacleCell)
            {
                StrategyCell = new StrategyCell(newStrategyCell);
                Board[newStrategyCell.X, newStrategyCell.Y] = Cells.strategyChange;
                isStrategyCellSet = true;
            }
        }
    }

    public Settings SetSettings(Settings settings)
    {
        if (settings == null)
        {
            return null;
        }

        // Consider Speed Setting
        if (settings.cellCount != 0)
        {
            //Width = settings.cellCount;
            //Height = settings.cellCount;
        }
        if (settings.isWall != null)
        {
            IsWall = true;

        }

        return new Settings(Width, IsWall, Speed.ToString());
    }

    public Coordinate GetInitalCoordinte(InitialPosition pos) =>
        pos.Equals(InitialPosition.UpLeft)
            ? new Coordinate(1, 1)
        : pos.Equals(InitialPosition.UpRight)
            ? new Coordinate(Width - 2, 1)
        : pos.Equals(InitialPosition.DownLeft)
            ? new Coordinate(1, Height - 2)
        : pos.Equals(InitialPosition.DownRight)
            ? new Coordinate(Width - 2, Height - 2)
        : throw new ArgumentException($" Invalid initial position {pos}");

    public bool ClearSnake(string playerName)
    {
        if (!Snakes.TryGetValue(playerName, out var snake))
        {
            return false;
        }

        if (snake == null)
        {
            return false;
        }

        foreach (var coord in snake.GetBodyList())
        {
            Board[coord.X, coord.Y] = Cells.empty;
        }

        return true;
    }

    public string PrepareForNewGame()
    {
        Scores = new ConcurrentDictionary<string, int>();
        if (!SetInitialPositionsAndActions())
        {
            return "Could not set initial positions";
        }

        //GenerateFood(true);
        //GenerateObstacles();
        GenerateStrategyCell();

        return string.Empty;
    }

    private bool SetInitialPositionsAndActions()
    {
        PendingActions.Clear();
        var allPositions = Enum.GetValues(typeof(InitialPosition)).Cast<InitialPosition>().ToArray();
        var allPlayers = Snakes.Keys.ToArray();
        string player;
        InitialPosition initPos;
        Coordinate initCoord;

        for (var i = 0; i < allPlayers.Length; i++)
        {
            player = allPlayers[i];
            initPos = allPositions[i];
            initCoord = GetInitalCoordinte(initPos);
            if (!Snakes.ContainsKey(player))
            {
                return false;
            }

            Snakes[player].SetInitialPosition(initCoord);
            Snakes[player].SetMovementStrategy(new DefaultMovementStrategy());

            _ = PendingActions.TryAdd(player, GetMoveDirection(initPos));
        }

        PreviousActions = new ConcurrentDictionary<string, MoveDirection>(PendingActions);
        return true;
    }

    public void UpdateActions()
    {
        var random = new Random();
        RefreshPendingActions();

        foreach (var snake in Snakes)
        {
            if (snake.Value == null || !snake.Value.IsActive)
            {
                continue;
            }

            if (!PendingActions.TryGetValue(snake.Key, out var currAction))
            {
                continue;
            }

            var translatedAction = currAction;

            if (snake.Value.GetMovementStrategy() is InverseMovementStrategy)
            {
                translatedAction = currAction switch
                {
                    MoveDirection.Left => MoveDirection.Right,
                    MoveDirection.Right => MoveDirection.Left,
                    MoveDirection.Up => MoveDirection.Down,
                    MoveDirection.Down => MoveDirection.Up,
                    _ => throw new ArgumentException($"Argument value of enum CoordDirection expected, but {currAction} found"),
                };
            }

            if (snake.Value.GetMovementStrategy() is ZigZagMovementStrategy)
            {
                translatedAction = currAction switch
                {
                    MoveDirection.Up or MoveDirection.Down => MoveDirection.Left,
                    MoveDirection.Left or MoveDirection.Right => MoveDirection.Up,
                    _ => throw new ArgumentException($"Argument value of enum CoordDirection expected, but {currAction} found"),
                };
            }

            var newHead = snake.Value.CloneHead();
            newHead.Update(translatedAction);

            if (snake.Value.GetMovementStrategy() is WrapAroundMovementStrategy)
            {
                newHead.X = (newHead.X + Width) % Width;
                newHead.Y = (newHead.Y + Height) % Height;
            }

            if (snake.Value.GetMovementStrategy() is InverseMovementStrategy || 
                snake.Value.GetMovementStrategy() is DefaultMovementStrategy || 
                snake.Value.GetMovementStrategy() is ZigZagMovementStrategy)
            {
                if (newHead.X < 0 || Width <= newHead.X || newHead.Y < 0 || Width <= newHead.Y)
                {
                    snake.Value.Deactivate();
                    continue;
                }
            }

            Tuple<Coordinate, Coordinate> moveResult;
            if (Board[newHead.X, newHead.Y].Equals(Cells.empty))
            {
                moveResult = snake.Value.Move(currAction, false);
                var currentScore = PointHandlerChain.Handle(this, snake, Cells.empty);
                snake.Value.UpdateCrownStage(currentScore);
            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.food))
            {
                Food = null;
                moveResult = snake.Value.Move(currAction, true);
                var currentScore = PointHandlerChain.Handle(this, snake, Cells.food);
                snake.Value.UpdateCrownStage(currentScore);
                //var currentScore = Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
                //snake.Value.UpdateCrownStage(currentScore);

            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.strategyChange))
            {
                StrategyCell = null;
                Board[newHead.X, newHead.Y] = Cells.empty;
                var randomChoice = random.Next(_strategiesCount);
                IMovementStrategy newMovementStrategy = randomChoice switch
                {
                    0 => new InverseMovementStrategy(),
                    1 => new WrapAroundMovementStrategy(Width, Height),
                    2 => new DefaultMovementStrategy(),
                    3 => new ZigZagMovementStrategy(),
                    _ => throw new Exception("Unexpected random choice for movement strategy"),
                };

                snake.Value.SetMovementStrategy(newMovementStrategy);

                moveResult = snake.Value.Move(currAction, false);
                var currentScore = PointHandlerChain.Handle(this, snake, Cells.strategyChange);
                snake.Value.UpdateCrownStage(currentScore);
                //var currentScore = Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
                //snake.Value.UpdateCrownStage(currentScore);

            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.snake))
            {
                var currentScore = PointHandlerChain.Handle(this, snake, Cells.snake);
                snake.Value.UpdateCrownStage(currentScore);
                if (snake.Value.GetMovementStrategy() is InverseMovementStrategy || snake.Value.GetMovementStrategy() is ZigZagMovementStrategy)
                {
                    moveResult = snake.Value.Move(currAction, false);
                    //var currentScore = Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
                    //snake.Value.UpdateCrownStage(currentScore);


                }
                else
                {
                    snake.Value.Deactivate();
                    continue;
                }
            }
            else
            {
                // Clone the snake using the PROTOTYPE pattern to revive it later
                var currentScore = PointHandlerChain.Handle(this, snake, Cells.obstacle);
                snake.Value.UpdateCrownStage(currentScore);

                snake.Value.Deactivate(); 
                clonedSnakes[snake.Key] = snake.Value.Clone();
                snake.Value.SetBodyToNull();
                ReviveSnake(snake.Key);

                continue;
            }

            if (moveResult == null)
            {
                continue;
            }

            if (Board[moveResult.Item1.X, moveResult.Item1.Y] == Cells.food)
            {
                Board[moveResult.Item1.X, moveResult.Item1.Y] = Cells.snake;
                Food = null;
            }
            else
            {
                Board[moveResult.Item1.X, moveResult.Item1.Y] = Cells.snake;
            }

            if (moveResult.Item2 != null)
            {   // snake tail must be removed
                Board[moveResult.Item2.X, moveResult.Item2.Y] = Cells.empty;
            }
        }
        GenerateFood(false);
        GenerateStrategyCell();
    }

    public void SetPendingAction(string player, MoveDirection direction)
    {
        if (PendingActions.TryGetValue(player, out var currDirection))
        {
            _ = PendingActions.TryUpdate(player, direction, currDirection);
        }
    }

    /// <summary>
    /// Sets current pending actions to snakes
    /// </summary>
    private void RefreshPendingActions()
    {
        foreach (var snake in Snakes)
        {
            // check if action is valid (get head and update and check
            var currentPendingAction = PendingActions[snake.Key];
            // if current action is not valid, set to last action. The very first action is always valid.
            if (snake.Value.IsDirectionToSelf(currentPendingAction))
            {
                PendingActions[snake.Key] = PreviousActions[snake.Key];
            }
        }

        PreviousActions = new ConcurrentDictionary<string, MoveDirection>(PendingActions);
    }

    public static MoveDirection GetMoveDirection(InitialPosition pos) =>
        pos switch
        {
            InitialPosition.UpLeft => MoveDirection.Down,
            InitialPosition.UpRight => MoveDirection.Left,
            InitialPosition.DownLeft => MoveDirection.Right,
            InitialPosition.DownRight => MoveDirection.Up,
            _ => MoveDirection.None,
        };

    public int GetScore(string playerName) {
        if (Scores.TryGetValue(playerName, out var score))
        {
            return score;
        }
        return 0;
    }

    public Dictionary<string, int> GetScores() {
        return new Dictionary<string, int>(Scores);
    }

    public int AddScore(string snakeName, int score)
    {
        return Scores.AddOrUpdate(snakeName, score, (key, oldValue) => oldValue + score);
    }

    public int DeductScore(string snakeName)
    {
        return Scores.AddOrUpdate(snakeName, 0, (key, oldValue) => Math.Max(oldValue - 1, 0));
    }

    public int ResetScore(string snakeName)
    {
        return Scores.AddOrUpdate(snakeName, 0, (key, oldValue) => 0);
    }

    public int GetCurrentScore(string snakeName)
    {
        if (Scores.TryGetValue(snakeName, out var currentScore))
        {
            return currentScore;
        }

        return 0;
    }

    public void InitializePointHandlers()
    {
        var foodHandler = new FoodPointsHandler();
        var obstacleHandler = new ObstaclePointsHandler();
        var snakeHandler = new SnakePointsHandler();
        var strategyHandler = new StrategyChangePointsHandler();

        // Link the chain
        foodHandler.SetNext(obstacleHandler)
                   .SetNext(snakeHandler)
                   .SetNext(strategyHandler);

        PointHandlerChain = foodHandler; // Store the root of the chain
    }
}
