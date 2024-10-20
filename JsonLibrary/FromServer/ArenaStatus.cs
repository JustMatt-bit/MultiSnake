using System.Collections.Generic;

using Newtonsoft.Json;

namespace JsonLibrary.FromServer;

[JsonObject]
public class ArenaStatus
{
    public List<Snake> ActiveSnakes { get; set; }
    public List<string> DisabledSnakes { get; set; }
    public List<Snake> SnakesToRevive { get; set; }
    public XY food { get; set; }
    public obstacleXY[] obstacles { get; set; }

    public ArenaStatus(XY food, obstacleXY[] obstacles)
    {
        this.food = food;
        this.obstacles = obstacles;
        ActiveSnakes = new List<Snake>();
        DisabledSnakes = new List<string>();
        SnakesToRevive = new List<Snake>();
    }
    public void AddActiveSnake(Snake s) => ActiveSnakes.Add(s);
    public void AddDisabledSnake(string player) => DisabledSnakes.Add(player);
    public void AddSnakeToRevive(Snake s) => SnakesToRevive.Add(s);
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
    public List<XY> body { get; set; }

    public Snake(string player, string color, XY head, XY tail, List<XY> body, int score = 0, bool stripes = false)
    {
        this.player = player;
        this.color = color;
        this.head = head;
        this.tail = tail;
        this.body = body;
        this.score = score;
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