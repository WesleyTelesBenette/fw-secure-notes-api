using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;

namespace fw_secure_notes_api.Filters;

public class ParmatersValidateActionFilter : IAsyncActionFilter
{
    private readonly ActionResultService _result;
    public ParmatersValidateActionFilter(ActionResultService result)
    {
        _result = result;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var routeVars = context.RouteData.Values;

        var routeTitle = routeVars.GetValueOrDefault("title")?.ToString();
        var routePin = routeVars.GetValueOrDefault("pin")?.ToString();

        bool isTitleValid = (routeTitle != null) && (routeTitle.ToString()!.Length <= 25);
        bool isPinValid = (routePin != null) && (Regex.IsMatch(routePin.ToString()!, @"^[a-zA-Z0-9\-]*$"));

        if ((!isTitleValid) || (!isPinValid))
        {
            context.Result = _result.GetActionAuto(ActionResultService.Results.Bad);
            return;
        }

        await next();
    }
}
