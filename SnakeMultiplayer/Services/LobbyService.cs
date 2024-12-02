using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

using SnakeMultiplayer.Services.Composite;

using SnakeMultiplayer.Services.Strategies.Movement;

namespace SnakeMultiplayer.Services;

public interface ILobbyService : ISubject
{
    LobbyStates State { get; }
    Speed Speed { get; }
    string ID { get; }
    public bool IsNoSpeed { get; }

    int GetPlayerCount();
    bool IsLobbyFull();
    bool IsActive();
    bool IsHost(string playerName);
    List<Player> GetAllPlayerStatus();
    void EndGame();
    ArenaStatus UpdateLobbyState();
    string AddPlayer(string playerName);
    void OnPlayerUpdate(string player, MoveDirection message);
    Settings SetSettings(Settings settings);
    ArenaStatus InitiateGameStart();
    public void SetState(ILobbyState newState);
    List<Player> RemovePlayer(string playerName);
    void RecordPlayerScores(IScoringService scoringService);
}

public class LobbyService : ILobbyService
{
    private ILobbyState LobbyState { get; set; }
    public string ID { get; private set; }
    public LobbyStates State => LobbyState.getStateName();
    public bool IsTimer { get; private set; }
    public Speed Speed { get => Arena.Speed; }
    public bool IsNoSpeed { get => Arena.Speed == Speed.NoSpeed; }

    public ConcurrentDictionary<string, Snake> players = new();

    private List<IObserver> observers = new List<IObserver>();
    public IArenaFactory factory;

    public readonly ArenaDirector director;

    public Arena Arena;
    readonly int MaxPlayers;
    readonly string HostPlayer;

    public LobbyService(string id, string host, int maxPlayers, int level)
    {
        ID = id;
        HostPlayer = host;
        LobbyState = new IdleState();
        MaxPlayers = maxPlayers;
        factory = ArenaFactoryProvider.GetFactory(level);
        
        IArenaBuilder builder;

        if(level == 4){
            builder = new RandomArenaBuilder(factory);
        }else{
            builder = new StandardArenaBuilder(factory);
        }
       
        director = new ArenaDirector();

        director.SetBuilder(builder);

        int boardSize = 20;
        int obstacleCount = level * 5;
        Speed speed = factory.GetSpeed();

        var obstacleGroup = new ObstacleGroup();
        for (int i = 0; i < obstacleCount; i++)
        {
            Random Random = new Random();
            var obstacle = new SingleObstacle(new Coordinate(Random.Next(0, boardSize), Random.Next(0, boardSize)));
            obstacleGroup.Add(obstacle);
        }

        Arena = director.ConstructArena(players, boardSize, boardSize, obstacleGroup, speed);
    }
     public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void SetState(ILobbyState newState){
        LobbyState = newState;
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(string operation, string record)
    {
        var currentObservers = new List<IObserver>(observers);
        foreach (var observer in currentObservers)
        {
            observer.Update(operation, this);
        }
    }

    public string AddPlayer(string playerName)
    {
        return LobbyState.AddPlayer(this, playerName);
    }

    public bool IsHost(string playerName) => playerName == HostPlayer;

    public string CanJoin(string playerName) =>
        string.IsNullOrWhiteSpace(playerName)
            ? "Empty (null) player name."
        : !IsActive()
            ? $"Lobby {ID} is not active. Please join another lobby"
        : IsLobbyFull()
            ? "Lobby is full."
        : PlayerExists(playerName)
            ? $"Player {playerName} already exists in lobby"
        : string.Empty;

    public void EndGame()
    {
        LobbyState.EndGame(this);
    }

    public ArenaStatus UpdateLobbyState()
    {
        return LobbyState.UpdateLobbyState(this);
    }

    public bool IsGameEnd()
    {
        var activePlayers = players.Values.Where(player => player.IsActive);
        var playerCount = players.Count;


        if (1 < playerCount)
        {
            return activePlayers.Count() <= 1;
        }
        else if (playerCount == 0)
        {
            return true;
        }

        return false;
    }

    public int GetPlayerCount() => players.Count;

    public List<Player> GetAllPlayerStatus()
    {
        var list = new List<Player>(players.Count);
        foreach (var player in players)
        {
            var newPlayer = new Player
            {
                name = player.Key,
                color = player.Value.GetColorString()
            };
            newPlayer.type = newPlayer.name.Equals(HostPlayer) ? "Host" : "Player";
            list.Add(newPlayer);
        }
        return list;
    }

    public ArenaStatus InitiateGameStart()
    {
        return LobbyState.InitiateGameStart(this);
    }

    public void OnPlayerUpdate(string player, MoveDirection direction)
    {
        if (State.Equals(LobbyStates.inGame))
            Arena.SetPendingAction(player, direction);
    }

    public Settings SetSettings(Settings settings) => Arena.SetSettings(settings);

    public List<Player> RemovePlayer(string playerName)
    {
        if (playerName == null)
        {
            throw new ArgumentNullException(nameof(playerName), "Attempt to remove player with null string.");
        }

        if (!PlayerExists(playerName))
            return GetAllPlayerStatus();

        if (playerName == HostPlayer)
            return null;

        _ = Arena.ClearSnake(playerName);
        _ = players.TryRemove(playerName, out _);
        return GetAllPlayerStatus();
    }

    public bool PlayerExists(string playerName) =>
        playerName == null
            ? throw new ArgumentNullException(nameof(playerName))
        : players.ContainsKey(playerName);

    public bool IsLobbyFull() => MaxPlayers <= players.Count;

    //TODO: Implement 
    public bool IsActive() => true;

    public void RecordPlayerScores(IScoringService scoringService) {
        Dictionary<string, int> scores = Arena.GetScores();
        foreach (var score in scores)
        {
            scoringService.RecordScore(score.Key, score.Value);
        }
    }
}