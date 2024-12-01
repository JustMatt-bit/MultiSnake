using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IScoringService _scoreService;
    public IActionResult Index() => View();

    public HomeController(IScoringService scoreService)
    {
        _scoreService = scoreService;
    }
    
    [HttpPost]
    public IActionResult Error(string errorMessage =
        "An error has occured.\n We are already taking action to prevent this error from happening.")
    {
        ViewData["ErrorMessage"] = errorMessage;
        return View("Views/Home/Index.cshtml");
    }

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    public IActionResult Leaderboard()
    {
        var scores = _scoreService.GetAllUserScores();
        return View(scores);
    }
}