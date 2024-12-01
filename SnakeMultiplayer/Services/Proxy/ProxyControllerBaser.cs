using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public abstract class ProxyControllerBase : Controller
{
    private readonly ILoggerService _logger;

    protected ProxyControllerBase(ILoggerService logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string controllerName = context.RouteData.Values["controller"]?.ToString();
        string actionName = context.RouteData.Values["action"]?.ToString();
        var parameters = context.ActionArguments;

        string parameterInfo = string.Join(", ", parameters.Select(p => $"{p.Key}: {p.Value}"));
        string logMessage = $"Controller: {controllerName}, Action: {actionName}, Parameters: {parameterInfo}, Timestamp: {DateTime.UtcNow}";

        _logger.LogAction(logMessage);

        base.OnActionExecuting(context);
    }
}
