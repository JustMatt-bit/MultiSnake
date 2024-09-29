using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using SnakeMultiplayer.Models;
using SnakeMultiplayer.Services;

namespace SnakeMultiplayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel request, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please provide valid login information.";
                return View(request);
            }

            bool isValidUser = await _authService.LoginAsync(request.Username, request.Password);
            if (isValidUser)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };

                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid login attempt.";
            return View(request);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel request)
        {
            if (ModelState.IsValid)
            {
                bool isRegistered = await _authService.RegisterAsync(request.Username, request.Password);

                if (isRegistered)
                {
                    return RedirectToAction("Login", "Account");
                }

                ModelState.AddModelError("", "The username is already taken. Please choose another one.");
            }

            return View(request);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
