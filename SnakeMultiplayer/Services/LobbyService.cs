using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JsonLibrary.FromClient;
using JsonLibrary.FromServer;

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
    List<Player> RemovePlayer(string playerName);
    void RecordPlayerScores(IScoringService scoringService);
}

public class LobbyService : ILobbyService
{
    public string ID { get; private set; }
    public LobbyStates State { get; private set; }
    public bool IsTimer { get; private set; }
    public Speed Speed { get => Arena.Speed; }
    public bool IsNoSpeed { get => Arena.Speed == Speed.NoSpeed; }

    readonly ConcurrentDictionary<string, Snake> players = new();

    private List<IObserver> observers = new List<IObserver>();
    private IArenaFactory factory;

    private readonly ArenaDirector director;

    readonly Arena Arena;
    readonly int MaxPlayers;
    readonly string HostPlayer;

    public LobbyService(string id, string host, int maxPlayers, int level)
    {
        ID = id;
        HostPlayer = host;
        State = LobbyStates.Idle;
        MaxPlayers = maxPlayers;
        factory = ArenaFactoryProvider.GetFactory(level);
        
        IArenaBuilder builder;

        if(level == 4){
            builder = new RandomArenaBuilder(factory);
        }else{
            builder = new ArenaBuilder(factory);
        }
       
        director = new ArenaDirector();

        director.SetBuilder(builder);

        int boardSize = 20;
        int obstacleCount = level * 5;
        Speed speed = factory.GetSpeed();
        Arena = director.ConstructArena(players, boardSize, boardSize, obstacleCount, speed);
    }
     public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
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
        var reason = CanJoin(playerName);
        if (!string.IsNullOrWhiteSpace(reason))
        {
            return reason;
        }

        var snake = factory.CreateSnake(players, playerName, new DefaultMovementStrategy());
         
        if (!players.TryAdd(playerName, snake))
        {
            return "An error has occured. Please try again later.";
        }

        return string.Empty;
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
        State = LobbyStates.Idle;
    }

    public ArenaStatus UpdateLobbyState()
    {
        Arena.UpdateActions();
        if(Arena.GetScores().Any(score => score.Value >= 15)){
            Arena.GenerateReport();
            NotifyObservers("end", "end");
        }
        if (!IsGameEnd())
            return Arena.GenerateReport();
        
        State = LobbyStates.Idle;
        return null;
    }

    private bool IsGameEnd()
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
        if (!State.Equals(LobbyStates.Idle))
            throw new Exception($"Tried to initialise game start while lobby {ID} is in Idle state.");

        State = LobbyStates.Initialized;
        _ = Arena.PrepareForNewGame();
        State = LobbyStates.inGame;

        Debug.WriteLine($"Game initialised in {ID} lobby.");
        return Arena.GenerateReport();
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