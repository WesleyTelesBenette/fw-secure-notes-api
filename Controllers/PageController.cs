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

    [HttpGet("files")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetFileList([FromRoute] string title, [FromRoute] string pin)
    {
        ICollection<FileModel> fileList = await _page.GetFileList(title, pin);

        return Ok(fileList);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePage([FromRoute] string title, [FromBody] CreatePageDto newPage)
    {
        PageModel page = new
        (
            title,
            await _gnPin.Generate(title),
            BCrypt.Net.BCrypt.HashPassword(newPage.Password)
        );

        return ((page.Pin != null) && (await _page.CreatePage(page)))
            ? Created("", page)
            : StatusCode(500, "Ocorreu um erro inesperado no servidor.");
    }

    [HttpPut("theme")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> ChangePageTheme(
        [FromRoute] string title,
        [FromRoute] string pin,
        [FromBody] UpdatePageThemeDto newTheme)
    {
        return (await _page.UpdateTheme(title, pin, newTheme.Theme))
            ? Ok()
            : StatusCode(500, "Ocorreu um erro inesperado no servidor.");
    }

    [HttpDelete]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeletePage(
        [FromRoute] string title,
        [FromRoute] string pin, 
        [FromBody] DeletePageDto newDelete)
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
