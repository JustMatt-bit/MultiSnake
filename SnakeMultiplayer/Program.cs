using System;
using System.IO;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SnakeMultiplayer.Controllers;

using SnakeMultiplayer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(o => o.EnableEndpointRouting = false);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddSingleton<IGameServerService, GameServerService>();
builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<IScoringService, ScoringService>();

builder.Services.AddTransient<IServerHub, ServerHub>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<LobbyController>();
builder.Services.AddScoped<LobbyProxyController>();
builder.Services.AddSingleton<ICommandService, CommandService>();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.Cookie.Name = "AuthCookie";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    var usersFileName = builder.Configuration["UsersFileName"];
    if (!File.Exists(usersFileName))
    {
        File.Create(usersFileName).Dispose();
    }
    
    var scoresFileName = builder.Configuration["ScoresFileName"];
    if (!File.Exists(scoresFileName))
    {
        File.Create(scoresFileName).Dispose();
    }
}

app.UseWebSockets();
app.MapHub<LobbyHub>("/LobbyHub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "createLobbyProxy",
    pattern: "Lobby/CreateLobby",
    defaults: new { controller = "LobbyProxy", action = "CreateLobby" });


app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();