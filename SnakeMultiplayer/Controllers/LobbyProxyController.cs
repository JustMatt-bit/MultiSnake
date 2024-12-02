using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SnakeMultiplayer.Controllers;
using SnakeMultiplayer.Services;

public class LobbyProxyController : Controller
{
    private readonly IGameServerService _gameServer;
    private readonly IAuthService _authService;
    private readonly LobbyController _lobbyController;

    public LobbyProxyController(IGameServerService gameServer, IAuthService authService, LobbyController lobbyController)
    {
        _gameServer = gameServer;
        _authService = authService;
        _lobbyController = lobbyController;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLobby(string id = "", int level = 1)
    {
        var username = User.Identity.Name;

        // Check if the user is an admin using AuthService
        if (await _authService.IsAdmin(username))
        {
            // Forward the request to the real LobbyController to create the lobby
            _lobbyController.ControllerContext = new ControllerContext
            {
                HttpContext = this.HttpContext
            };

            return _lobbyController.CreateLobby(id, level);
        }
        else
        {
            ViewData["ErrorMessage"] = "Only administrators can create a lobby.";
            return View("Views/Home/Index.cshtml");
        }
    }
}