using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Controllers
{
    [Authorize]
    public class LobbyProxyController : Controller
    {
        private readonly IGameServerService _gameServer;
        private readonly IAuthService _authService;

        public LobbyProxyController(IGameServerService gameServer, IAuthService authService)
        {
            _gameServer = gameServer;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLobby(string id = "", int level = 1)
        {
            var username = User.Identity.Name;
            
            // Check if the user is an admin using AuthService
            if (await _authService.IsAdmin(username))
            {
                // Forward the request to the real LobbyController to create the lobby
                var lobbyController = new LobbyController(_gameServer)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = this.HttpContext
                    }
                };

                return lobbyController.CreateLobby(id, level);
            }
            else
            {
                ViewData["ErrorMessage"] = "Only administrators can create a lobby.";
                return View("Views/Home/Index.cshtml");
            }
        }
    }
}
