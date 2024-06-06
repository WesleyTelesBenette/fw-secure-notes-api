using fw_secure_notes_api.Attributes;
using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Models;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class PageController : Controller
{
    private readonly PageRepository _page;
    private readonly GeneratePinService _gnPin;
    private readonly ActionResultService _result;

    public PageController(PageRepository page, GeneratePinService gnPin, ActionResultService result)
    {
        _page = page;
        _gnPin = gnPin;
        _result = result;
    }

    [HttpGet("themes")]
    [NoParameters]
    public IActionResult GetThemeList()
    {
        try
        {
            var values = Enum.GetValues(typeof(ThemePage)).Cast<ThemePage>().ToList();
            var themeListLength = values.Count;

            Dictionary<byte, string> themeList = [];

            for (byte c = 0; c < themeListLength; c++)
                themeList.Add(c, values[c].ToString());

            return _result.GetAction(ActionResultService.Results.Get, content: themeList);
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpGet("theme")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetPageTheme([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var theme = await _page.GetPageTheme(title, pin);

            return (theme != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: theme)
                : _result.GetActionAuto(ActionResultService.Results.NotFound, "Page.Theme");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpGet("files")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public IActionResult GetFileList([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var fileList = _page.GetFileList(title, pin);

            return (fileList != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: fileList)
                : _result.GetActionAuto(ActionResultService.Results.NoContent, "Page.Files");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPost]
    [NoParameters]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageDto newPage)
    {
        try
        {
            PageModel newPageModel = new()
            {
                Title = newPage.Title,
                Pin = await _gnPin.Generate(newPage.Title),
                Password = BCrypt.Net.BCrypt.HashPassword(newPage.Password)
            };

            if (string.IsNullOrEmpty(newPageModel.Pin))
                throw new Exception("Falha ao gerar PIN.");

            var page = await _page.CreatePage(newPageModel);

            return _result.GetActionAuto(page, "Page");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("theme")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePageTheme
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePageThemeDto newTheme)
    {
        try
        {
            var result = await _page.UpdateTheme(title, pin, newTheme.Theme);

            return _result.GetActionAuto(result, "Page.Theme");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("password")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePagePassword
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePagePasswordDto updatePassword)
    {
        try
        {
            var result = await _page.UpdatePassword(title, pin, updatePassword.OldPassword, updatePassword.NewPassword);

            return _result.GetActionAuto(result, "Page.Password");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpDelete]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeletePage
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] DeletePageDto newDelete)
    {
        try
        {
            var result = await _page.DeletePage(title, pin, newDelete.Password);

            return _result.GetActionAuto(result, "Page");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }
}
