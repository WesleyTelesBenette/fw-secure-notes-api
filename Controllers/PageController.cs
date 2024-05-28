using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Models;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(TokenValidateActionFilter))]
public class PageController : Controller
{
    private readonly PageRepository _page;
    private readonly GeneratePin _gnPin;

    public PageController(PageRepository page, GeneratePin gnPin)
    {
        _page = page;
        _gnPin = gnPin;
    }

    [HttpGet("files")]
    public async Task<IActionResult> GetFileList([FromRoute] string title, [FromRoute] string pin)
    {
        var list = await _page.GetFileList(title, pin);
        return (list != null)
            ? Ok()
            : StatusCode(500);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePage([FromRoute] string title, [FromBody] CreatePageDto newPage)
    {
        string pin = await _gnPin.Generate(title);
        PageModel page = new(title, pin, newPage.Password);

        return ((pin != null) && (await _page.CreatePage(page)))
            ? Created("", page)
            : StatusCode(500);
    }

    [HttpPut]
    public async Task<IActionResult> ChangePageTheme(
        [FromRoute] string title,
        [FromRoute] string pin,
        [FromBody] UpdatePageThemeDto newTheme)
    {
        return (await _page.UpdateTheme(title, pin, newTheme.Theme))
            ? Ok()
            : StatusCode(500);
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePage(
        [FromRoute] string title,
        [FromRoute] string pin, 
        [FromBody] DeletePageDto newDelete)
    {

        if (await _page.IsPageValid(title, pin, newDelete.Password))
        {
            return (await _page.DeletePage(title, pin, newDelete.Password))
                ? Ok()
                : StatusCode(500);
        }

        return Unauthorized();
    }


    /*
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    */
}
