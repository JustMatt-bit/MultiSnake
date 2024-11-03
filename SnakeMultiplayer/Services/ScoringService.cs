using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SnakeMultiplayer.Models;

namespace SnakeMultiplayer.Services
{
    public interface IScoringService
    {
        void RecordScore(string playerId, int score);
        List<int> GetScores(string playerId);
        IEnumerable<UserScore> GetAllUserScores();
    }

    public class ScoringService : IScoringService
    {
        // Tracks scores of all players in recorded games
        private Dictionary<string, List<int>> _playerScores;
        private readonly string _scoresFile;

        public ScoringService(IConfiguration configuration)
        {
            _playerScores = new Dictionary<string, List<int>>();
            _scoresFile = configuration["ScoresFileName"];

            var lines = File.ReadAllLines(_scoresFile);
            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var playerId = parts[0];
                    if (int.TryParse(parts[1], out int score))
                    {
                        if (!_playerScores.ContainsKey(playerId))
                        {
                            _playerScores[playerId] = new List<int>();
                        }
                        _playerScores[playerId].Add(score);
                    }
                }
            }
        }

        public void RecordScore(string playerId, int score)
        {
            if (!_playerScores.ContainsKey(playerId))
            {
                _playerScores[playerId] = new List<int>();
            }

            if (score <= 0) return;

            _playerScores[playerId].Add(score);

            using (StreamWriter writer = new(_scoresFile, append: true))
            {
                writer.WriteLine($"{playerId}:{score}");
            }
        }

        public List<int> GetScores(string playerId)
        {
            if (_playerScores.ContainsKey(playerId))
            {
                return _playerScores[playerId];
            }
            return null;
        }

        public IEnumerable<UserScore> GetAllUserScores()
        {
            var scores = new List<UserScore>();
            foreach (var player in _playerScores)
            {
                List<int> playerScores = GetScores(player.Key);
                if (playerScores == null) continue;
                    
                scores.Add(new UserScore(player.Key, playerScores.Max()));
            }
            
            return scores.OrderByDescending(s => s.HighestScore);
        }
    }
}