﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Controllers;

[Authorize]
public class LobbyController : Controller
{

    private static readonly string InvalidStringErrorMessage = @"Please use only letters, numbers and spaces only between words. ";

    [HttpGet]
    public IActionResult Index() => View("Views/Lobby/CreateLobby.cshtml");

    [HttpGet]
    public IActionResult CreateLobby() => View();

    [HttpPost]
    public IActionResult CreateLobby([FromServices] IGameServerService gameServer, string id = "", int level = 1)
    {
        string playerName = User.Identity.Name;
        ViewData["playerName"] = playerName;
        ViewData["lobbyId"] = id;
        ViewData["level"] = level;

        var errorMessage = IsValid(id, playerName);
        if (!errorMessage.Equals(string.Empty))
        {
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }

        if (!gameServer.TryCreateLobby(id, playerName, gameServer, level))
        {
            ViewData["ErrorMessage"] = $"Lobby with {id} already exists. Please enter different name";
            return View();
        }

        SetCookie("PlayerName", playerName);
        SetCookie("LobbyId", id);
        ViewData["IsHost"] = true;

        return View("Views/Lobby/Index.cshtml");
    }

    [HttpGet]
    public IActionResult JoinLobby(string id = "")
    {
        ViewData["lobbyId"] = id;
        return View();
    }

    [HttpPost]
    public IActionResult JoinLobby([FromServices] IGameServerService gameServer, string id = "")
    {
        string playerName = User.Identity.Name;
        ViewData["playerName"] = playerName;
        ViewData["lobbyId"] = id;

        var errorMessage = IsValid(id, playerName);
        if (!errorMessage.Equals(string.Empty))
        {
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }

        errorMessage = gameServer.CanJoin(id, playerName);
        if (string.IsNullOrEmpty(errorMessage))
        {
            SetCookie("PlayerName", playerName);
            SetCookie("LobbyId", id);
            return View("Views/Lobby/Index.cshtml");
        }
        else
        {
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }
    }

    [HttpGet]
    public IActionResult Status([FromServices] IGameServerService gameServer)
    {
        ViewData["Lobbies"] = gameServer.GetLobbyStatus();

        return View();
    }

    private static string IsValid(string lobbyName, string playerName) =>
        string.IsNullOrEmpty(playerName)
            ? "Please enter your player name"
        : string.IsNullOrEmpty(lobbyName)
            ? "Please enter lobby name"
        : !GameServerService.ValidStringRegex.IsMatch(playerName)
            ? "Player name is incorrect.\n" + InvalidStringErrorMessage
        : !GameServerService.ValidStringRegex.IsMatch(lobbyName)
            ? "Lobby name is incorrect.\n" + InvalidStringErrorMessage
        : string.Empty;

    private void SetCookie(string name, string value)
    {
        var options = new CookieOptions()
        {
            IsEssential = true,
        };
        Response.Cookies.Append(name, value, options);
    }
}