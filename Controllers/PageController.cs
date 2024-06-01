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

    public PageController(PageRepository page, GeneratePinService gnPin)
    {
        _page = page;
        _gnPin = gnPin;
    }

    [HttpGet("themes")]
    [NoParameters]
    public IActionResult GetThemeList()
    {
        var values = Enum.GetValues(typeof(ThemePage)).Cast<ThemePage>().ToList();
        var themeListLength = values.Count;

        Dictionary<byte, string> themeList = [];

        for (byte c = 0; c < themeListLength; c++)
            themeList.Add(c, values[c].ToString());
        
        return Ok(new { themes = themeList });
    }

    [HttpGet("theme")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetPageTheme([FromRoute] string title, [FromRoute] string pin)
    {
        var theme = await _page.GetPageTheme(title, pin);

        return Ok(theme);
    }

    [HttpGet("files")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetFileList([FromRoute] string title, [FromRoute] string pin)
    {
        ICollection<FileModel> fileList = await _page.GetFileList(title, pin);

        return Ok(fileList);
    }

    [HttpPost]
    [NoParameters]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageDto newPage)
    {
        PageModel page = new
        (
            newPage.Title,
            await _gnPin.Generate(newPage.Title),
            BCrypt.Net.BCrypt.HashPassword(newPage.Password)
        );

        return ((page.Pin != null) && (await _page.CreatePage(page)))
            ? Created("", page)
            : StatusCode(500, "Ocorreu um erro inesperado no servidor.");
    }

    [HttpPut("theme")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePageTheme
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePageThemeDto newTheme)
    {
        return (await _page.UpdateTheme(title, pin, newTheme.Theme))
            ? Ok()
            : StatusCode(500, "Ocorreu um erro inesperado no servidor.");
    }

    [HttpPut("password")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePagePassword
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] UpdatePagePasswordDto newPassword)
    {
        if (await _page.IsPageValid(title, pin, newPassword.OldPassword))
        {
            return (await _page.UpdatePassword(title, pin, newPassword.NewPassword))
                ? Ok()
                : StatusCode(500, "Ocorreu um erro inesperado no servidor.");
        }

        return Unauthorized();
    }

    [HttpDelete]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeletePage
        ([FromRoute] string title, [FromRoute] string pin, [FromBody] DeletePageDto newDelete)
    {
        if (await _page.IsPageValid(title, pin, newDelete.Password))
        {
            return (await _page.DeletePage(title, pin))
                ? Ok()
                : StatusCode(500);
        }

        return Unauthorized();
    }
}
