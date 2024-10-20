using System.Collections.Generic;

using Newtonsoft.Json;

namespace JsonLibrary.FromServer;

[JsonObject]
public class ArenaStatus
{
    public List<Snake> ActiveSnakes { get; set; }
    public List<string> DisabledSnakes { get; set; }
    public XY food { get; set; }
    public obstacleXY[] obstacles { get; set; }
    public StrategyCellXY StrategyCell { get; set; }

    public ArenaStatus(XY food, obstacleXY[] obstacles, StrategyCellXY strategyCell)
    {
        this.food = food;
        this.obstacles = obstacles;
        ActiveSnakes = new List<Snake>();
        DisabledSnakes = new List<string>();
        StrategyCell = strategyCell;
    }
    public void AddActiveSnake(Snake s) => ActiveSnakes.Add(s);

    public void AddDisabledSnake(string player) => DisabledSnakes.Add(player);

    public static Players Deserialize(string json) => JsonConvert.DeserializeObject<Players>(json);
    public static string Serialize(Players m) => JsonConvert.SerializeObject(m);
}

[JsonObject]
public class Snake
{
    public string player { get; set; }
    public string color { get; set; }
    public int score { get; set; }
    public XY head { get; set; }
    public XY tail { get; set; }
    public bool isStriped { get; set; }
    public string movementStrategy { get; set; }

    public Snake(string player, string color, string movementStrategy, XY head, XY tail, int score = 0, bool stripes = false)
    {
        this.player = player;
        this.color = color;
        this.head = head;
        this.tail = tail;
        this.score = score;
        this.movementStrategy = movementStrategy;
        this.isStriped = stripes;
    }
}

[JsonObject]
public class obstacleXY
{
    public XY position { get; set; }
    public string color { get; set; }
    public obstacleXY(XY position, string color)
    {
        this.position = position;
        this.color = color;
    }
}

[JsonObject]
public class StrategyCellXY
{
    public XY Position { get; set; }
    public string Color { get; set; }
    public StrategyCellXY(XY position, string color)
    {
        this.Position = position;
        this.Color = color;
    }
}

[JsonObject]
public class XY
{
    public int x { get; set; }
    public int y { get; set; }
    public XY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}