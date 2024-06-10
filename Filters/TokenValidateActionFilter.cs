using fw_secure_notes_api.Data;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace fw_secure_notes_api.Filters;

public class TokenValidateActionFilter : IAsyncActionFilter
{
    private readonly PageRepository _page;
    private readonly ActionResultService _result;

    public TokenValidateActionFilter(PageRepository page, ActionResultService result)
    {
        _page = page;
        _result = result;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user      = context.HttpContext.User;
        var routeVars = context.RouteData.Values;

        var routeTitle = routeVars.GetValueOrDefault("title")?.ToString();
        var routePin   = routeVars.GetValueOrDefault("pin")?.ToString();

        var tokenTitle = user.Claims.FirstOrDefault(c => c.Type == "Title")?.Value ?? "";
        var tokenPin   = user.Claims.FirstOrDefault(c => c.Type == "Pin")?.Value ?? "";

        if (!await IsPageExist(routeTitle, routePin))
        {
            context.Result = _result.GetActionAuto(ActionResultService.Results.NotFound);
            return;
        }

        if ((!await IsPublicPage(routeTitle!, routePin!))
            && (!IsAuthorizedPage(user, routeTitle!, routePin!, tokenTitle!, tokenPin!)))
        {
            context.Result = _result.GetActionAuto(ActionResultService.Results.Unauthorized);
            return;
        }

        await next();
    }

    public async Task<bool> IsPageExist(string? routeTitle, string? routePin)
    {
        return
            (!string.IsNullOrEmpty(routeTitle))
            && (!string.IsNullOrEmpty(routePin))
            && (await _page.IsPageExist(routeTitle, routePin));
    }

    private async Task<bool> IsPublicPage(string title, string pin)
    {
        return (!await _page.IsPageHasPassword(title, pin));
    }

    private static bool IsAuthorizedPage(ClaimsPrincipal user, string routeTitle, string routePin, string tokenTitle, string tokenPin)
    {
        return
            (user.Identity != null)
            && (user.Identity.IsAuthenticated)
            && (tokenTitle == routeTitle)
            && (tokenPin == routePin);
    }
}
