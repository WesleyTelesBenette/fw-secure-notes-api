using fw_secure_notes_api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

        if ((user.Identity != null) && (user.Identity.IsAuthenticated))
        {
            var tokenTitle = user.Claims.FirstOrDefault(c => c.Type == "title")?.Value ?? "";
            var tokenPin = user.Claims.FirstOrDefault(c => c.Type == "pin")?.Value ?? "";
            var token = $"{tokenTitle}-{tokenPin}";

            var routeTitle = routeVars.GetValueOrDefault("title") ?? "";
            var routePin = routeVars.GetValueOrDefault("pin") ?? "";
            var route = $"{routeTitle}-{routePin}";


            if ((token == route) && (await _page.IsPageExist(tokenTitle, tokenPin)))
            {
                await next();
                return;
            }
        }

        context.Result = new UnauthorizedResult();
        return;
    }
}

