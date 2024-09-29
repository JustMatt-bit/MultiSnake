using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using BC = BCrypt.Net.BCrypt;

namespace SnakeMultiplayer.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string userName, string password);
        Task<bool> RegisterAsync(string userName, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly string usersFileName;

        public AuthService(IConfiguration configuration)
        {
            usersFileName = configuration["UsersFileName"];
        }

        public async Task<bool> RegisterAsync(string userName, string password)
        {
            if (await UserExistsAsync(userName))
            {
                return false;
            }

            var hashedPassword = BC.HashPassword(password);
            var userEntry = $"{userName}:{hashedPassword}";

            await File.AppendAllTextAsync(usersFileName, userEntry + "\n");

            return true;
        }

        public async Task<bool> LoginAsync(string userName, string password)
        {
            var user = await GetUserAsync(userName);
            if (user == null)
            {
                return false;
            }

            var storedHashedPassword = user.Split(':')[1];
            var isPasswordValid = BC.Verify(password, storedHashedPassword);

            return isPasswordValid;
        }

        private async Task<bool> UserExistsAsync(string userName)
        {
            var lines = await File.ReadAllLinesAsync(usersFileName);
            foreach (var line in lines)
            {
                var user = line.Split(':')[0];
                if (user == userName)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<string> GetUserAsync(string userName)
        {
            var lines = await File.ReadAllLinesAsync(usersFileName);
            foreach (var line in lines)
            {
                var user = line.Split(':')[0];
                if (user == userName)
                {
                    return line; 
                }
            }
            return null; 
        }
    }
}
