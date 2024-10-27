using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services;

public class Arena
{
    public Speed Speed { get;  set; }

    readonly Random Random = new(Guid.NewGuid().GetHashCode());
    readonly ConcurrentDictionary<string, Snake> Snakes;
    private Dictionary<string, Snake> clonedSnakes = new Dictionary<string, Snake>();

    readonly ConcurrentDictionary<string, MoveDirection> PendingActions;
    readonly int _strategiesCount = 4;

    ConcurrentDictionary<string, int> Scores;
    ConcurrentDictionary<string, MoveDirection> PreviousActions;
    Cells[,] Board;
    public int Width;
    public int Height;
    bool IsWall;
    Coordinate Food;
    LinkedList<Obstacle> Obstacles;
    StrategyCell StrategyCell;

    public Arena(ConcurrentDictionary<string, Snake> players, Speed speed)
    {
        Snakes = players;
        PendingActions = new ConcurrentDictionary<string, MoveDirection>();
        Scores = new ConcurrentDictionary<string, int>();
        Food = null;
        Obstacles = new LinkedList<Obstacle>();
        Speed = speed;
    }

    public void CreateBoard(int width, int height) {
        Width = width;
        Height = height;
        Board = new Cells[Width, Height];
    }

    public ArenaStatus GenerateReport()
    {
        obstacleXY[] obstaclesArray = Obstacles.Select(o => new obstacleXY(new XY(o.Position.X, o.Position.Y), o.Color)).ToArray();

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
                var score = GetScore(snake.Key);
                var isStriped = snake.Value.IsStriped;
                var body = snake.Value.GetBodyAsCoordinateList().Select(coord => coord.ConvertToXY()).ToList(); // Convert to List<XY>
                var currentMovingStrategy = snake.Value.GetMovementStrategy().ToString();
                var tempSnake = new JsonLibrary.FromServer.Snake(snake.Key, color, currentMovingStrategy, head, tail, body, score, isStriped);
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
                var currentMovingStrategy = snake.Value.GetMovementStrategy().ToString();
                var tempSnake = new JsonLibrary.FromServer.Snake(snake.Key, color, currentMovingStrategy, head, tail, null, score, isStriped);
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

        // Set the obstacle position and update the board
        Obstacles.AddLast(new Obstacle(obstaclePosition));
        Board[obstaclePosition.X, obstaclePosition.Y] = Cells.obstacle;
    }

     public void AddObstacles(Coordinate[] obstaclePosition) {
        for(int n = 0; n < obstaclePosition.Count(); n++){
            if (obstaclePosition[n] == null)
        {
            throw new ArgumentNullException(nameof(obstaclePosition));
        }

        // Ensure the obstacle position is within the bounds of the arena
        if (obstaclePosition[n].X < 0 || obstaclePosition[n].X >= Width || obstaclePosition[n].Y < 0 || obstaclePosition[n].Y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(obstaclePosition), "Obstacle position is out of bounds.");
        }

        // Set the obstacle position and update the board
        Obstacles.AddLast(new Obstacle(obstaclePosition[n]));
        Board[obstaclePosition[n].X, obstaclePosition[n].Y] = Cells.obstacle;
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
            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.food))
            {
                Food = null;
                moveResult = snake.Value.Move(currAction, true);
                Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
                Console.WriteLine($"Player {snake.Key} scored. Current score: {Scores[snake.Key]}");
            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.strategyChange))
            {
                StrategyCell = null;
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
                Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
            }
            else if (Board[newHead.X, newHead.Y].Equals(Cells.snake))
            {
                if (snake.Value.GetMovementStrategy() is InverseMovementStrategy || snake.Value.GetMovementStrategy() is ZigZagMovementStrategy)
                {
                    moveResult = snake.Value.Move(currAction, false);
                    Scores.AddOrUpdate(snake.Key, 1, (key, oldValue) => oldValue + 1);
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
}
