namespace SnakeMultiplayer.Services;

public enum MoveDirection
{
    None = 0,
    Up = 1,
    Right = 2,
    Down = 3,
    Left = 4,
}

public enum Speed
{
    NoSpeed = 0,
    Fast = 1,
    Normal = 2,
    Slow = 3
}

public enum InitialPosition
{
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
}

public enum Cells
{
    empty = 0,
    food = 1,
    snake = 2,
    obstacle = 3,
    strategyChange = 4
}


public enum Shapes
{
    circle,
    square,
    triangle,
    ellipse,
    polygon
}

public enum PlayerColor
{
    greenyellow,
    dodgerblue,
    orange,
    mediumpurple,
    crimson,
    coral,
    gold,
    lightseagreen,
    slateblue,
    tomato,
    lavender,
    mediumseagreen,
    darkorange,
    lightcoral,
    plum,
    palevioletred,
    teal,
    lightblue,
    forestgreen,
    darkviolet
}

public enum LobbyStates
{
    Idle,
    Initialized,
    inGame,
    closed,
}