using fw_secure_notes_api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
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
        var user      = context.HttpContext.User;
        var routeVars = context.RouteData.Values;

        var routeTitle = routeVars.GetValueOrDefault("title")?.ToString();
        var routePin   = routeVars.GetValueOrDefault("pin")?.ToString();

        var tokenTitle = user.Claims.FirstOrDefault(c => c.Type == "title")?.Value;
        var tokenPin   = user.Claims.FirstOrDefault(c => c.Type == "pin")?.Value;

        if (!IsRequestPost(context))
        {
            if (!await IsPageExist(routeTitle, routePin))
            {
                context.Result = new NotFoundResult();
                return;
            }

            if ((!await IsPublicPage(user, routeTitle!, routePin!))
                && (!IsAuthorizedPage(user, routeTitle!, routePin!, tokenTitle!, tokenPin!)))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        await next();
    }

    private static bool IsRequestPost(ActionExecutingContext context)
    {
        return (context.HttpContext.Request.Method == HttpMethods.Post);
    }

    public async Task<bool> IsPageExist(string? routeTitle, string? routePin)
    {
        return
            (!string.IsNullOrEmpty(routeTitle))
            && (!string.IsNullOrEmpty(routePin))
            && (await _page.IsPageExist(routeTitle, routePin));
    }

    private async Task<bool> IsPublicPage(ClaimsPrincipal user, string title, string pin)
    {
        return
            (user.Identity == null)
            && (!await _page.IsPageHasPassword(title, pin));
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
