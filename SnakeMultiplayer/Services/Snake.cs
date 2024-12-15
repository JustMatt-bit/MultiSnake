using System;
using System.Collections.Generic;
using System.Linq;

using SnakeMultiplayer.Services.Strategies.Movement;
using SnakeMultiplayer.Services.Appearance;
using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services.Decorator;
using SnakeMultiplayer.Services.Memento;

namespace SnakeMultiplayer.Services;

public class Snake : IPrototype<Snake>
{
    private LinkedList<Coordinate> body;
    public IMovementStrategy MovementStrategy { get; private set; }
    public SnakeAppearance Appearance { get; private set; }
    public readonly PlayerColor color;
    public CrownStage CrownStage { get; private set; }

    public string shape { get; private set; }
    public bool IsStriped { get; private set; }
    public bool IsRevived { get; set; }
    public bool IsActive { get; private set; }
    public Coordinate Tail { get; private set; }
    public object Player { get; internal set; }


    public Snake(bool stripes, IMovementStrategy strategy, SnakeAppearance appearance)
    {   
        Appearance = appearance;
        body = new LinkedList<Coordinate>();
        IsStriped = stripes;
        MovementStrategy = strategy;
    }


    public Snake Clone() 
    {
        var clonedSnake = (Snake)this.MemberwiseClone();
        clonedSnake.body = new LinkedList<Coordinate>(this.body.Select(coord => coord.Clone()));
        return clonedSnake;
    }

    public void SetInitialPosition(Coordinate coordinate)
    {
        IsActive = true;
        body = new LinkedList<Coordinate>();
        _ = body.AddFirst(coordinate);
    }


    public void SetBodyToNull(){
        body = null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public Tuple<Coordinate, Coordinate> Move(MoveDirection direction, bool isFood)
    {
        if (body == null)
        {
            _ = new Tuple<Coordinate, Coordinate>(null, null);
        }

        var result = MovementStrategy.Move(body, Tail, direction, isFood);
        Tail = result.Item2;

        return new Tuple<Coordinate, Coordinate>(CloneHead(result.Item1), Tail);
    }
    /// <summary>
    /// Check whether direction is valid.
    /// (Snake always can not move backwards).
    /// </summary>
    public bool IsDirectionToSelf(MoveDirection direction)
    {
        if (body == null || body.Count <= 1)
        {
            return false;
        }

        var head = CloneHead();
        head.Update(direction);
        return body.First.Next.Value.Equals(head);
    }

    public Coordinate CloneHead() =>
        body == null || body.First.Value == null
        ? null
        : body.First.Value.Clone();

    public Coordinate CloneHead(LinkedList<Coordinate> body) =>
    body == null || body.First.Value == null
    ? null
    : body.First.Value.Clone();


    public List<Coordinate> GetCoordinates() => body?.ToList<Coordinate>();

    public bool Contains(Coordinate food)
    {
        if (body == null)
        {
            return false;
        }

        foreach (var coord in body)
        {
            if (coord.Equals(food))
            {
                return true;
            }
        }

        return false;
    }

    public List<string> ToStringBody()
    {
        var body = new List<string>(this.body.Count);

        foreach (var coord in this.body)
        {
            body.Add(coord.ToString());
        }
        return body;
    }

    public List<Coordinate> GetBodyList()
    {
        var list = new List<Coordinate>(body.Count);
        foreach (var coord in body)
        {
            list.Add(coord.Clone());
        }
        return list;
    }

    public string GetColorString() => Enum.GetName(typeof(PlayerColor), Appearance.Color);

    public Coordinate[] GetBodyArray(){
        return body.ToArray();
    }

    public List<Coordinate> GetBodyAsCoordinateList()
    {
        return body.Select(coord => coord.Clone()).ToList();
    }

    public void SetMovementStrategy(IMovementStrategy strategy)
    {
        MovementStrategy = strategy;
    }

    public IMovementStrategy GetMovementStrategy() => MovementStrategy;

    public void UpdateCrownStage(int score)
    {
        if (score >= 10)
            this.CrownStage = CrownStage.FullWithJewels;
        else if (score >= 5)
            this.CrownStage = CrownStage.Colored;
        else if (score >= 1)
            this.CrownStage = CrownStage.Outline;
        else
            this.CrownStage = CrownStage.None;
    }

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }

    public void ToggleActiveState()
    {
        this.IsActive = !this.IsActive;
    }

    public ISnakeMemento SaveState()
    {
        if (body == null)
        {
            throw new InvalidOperationException("Cannot save state; body is null.");
        }

        return new SnakeBodyMemento(body, Tail, MovementStrategy);
    }

    public void RestoreState(ISnakeMemento memento)
    {
        if (memento == null)
        {
            throw new ArgumentNullException(nameof(memento), "Memento cannot be null.");
        }

        var bodyState = memento.GetBodyState();
        this.body = bodyState.BodyCoordinates;
        this.Tail = bodyState.TailCoordinates;
        this.MovementStrategy = bodyState.CurrentSnakeMovementStrategy;
    }

}

