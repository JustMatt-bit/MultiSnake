namespace SnakeMultiplayer.Models
{
    public class UserScore
    {
        public string Username { get; set; }
        public int HighestScore { get; set; }

        public UserScore(string username, int highestScore)
        {
            Username = username;
            HighestScore = highestScore;
        }
    }
}