using fw_secure_notes_api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace fw_secure_notes_api.Filters;

public class TokenValidateActionFilter : IAsyncActionFilter
{
    private readonly PageRepository _page;

    public TokenValidateActionFilter(PageRepository page)
    {
        _page = page;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var routeVars = context.RouteData.Values;

        var routeTitle = routeVars.GetValueOrDefault("title")?.ToString();
        var routePin = routeVars.GetValueOrDefault("pin")?.ToString();

        var tokenTitle = user.Claims.FirstOrDefault(c => c.Type == "title")?.Value ?? "";
        var tokenPin = user.Claims.FirstOrDefault(c => c.Type == "pin")?.Value ?? "";

        if (!await IsPageExist(routeTitle, routePin))
        {
            context.Result = new NotFoundResult();
            return;
        }

        if ((await IsPublicPage(user, routeTitle!, routePin!)) ||
            (IsAuthorizedPage(user, routeTitle!, routePin!, tokenTitle, tokenPin)))
        {
            await next();
            return;
        }

        context.Result = new UnauthorizedResult();
    }

    public async Task<bool> IsPageExist(string? routeTitle, string? routePin)
    {
        if ((!string.IsNullOrEmpty(routeTitle)) && (!string.IsNullOrEmpty(routePin)))
        {
            return await _page.IsPageExist(routeTitle, routePin);
        }

        return false;
    }

    private async Task<bool> IsPublicPage(ClaimsPrincipal user, string title, string pin)
    {
        if (user.Identity == null)
        {
            return await _page.IsPageHasPassword(title, pin);
        }

        return false;
    }

    private static bool IsAuthorizedPage(ClaimsPrincipal user, string routeTitle, string routePin, string tokenTitle, string tokenPin)
    {
        return
            (user.Identity != null) &&
            (user.Identity.IsAuthenticated) &&
            (tokenTitle == routeTitle) &&
            (tokenPin == routePin);
    }
}
