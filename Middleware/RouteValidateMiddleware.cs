using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fw_secure_notes_api.Middleware;

public class RouteValidateMiddleware
{
    private readonly RequestDelegate _next;

    public RouteValidateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var routeVars = context.Request.RouteValues;

        var routeTitle = routeVars.GetValueOrDefault("title");
        var routePin = routeVars.GetValueOrDefault("pin");

        bool isTitleValid = (routeTitle != null) && (routeTitle.ToString()!.Length <= 25);
        bool isPinValid   = (routePin != null)   && (Regex.IsMatch(routePin.ToString()!, @"^[a-zA-Z0-9\-]*$") == true);

        if ((!isTitleValid) || (!isPinValid))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Bad Request");
            context.Abort();
            return;
        }

        await _next(context);
    }
}
