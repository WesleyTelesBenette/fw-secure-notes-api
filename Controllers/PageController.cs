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
        var listFiles = await _page.GetFileList(title, pin);

        return (listFiles.Count > 0)
            ? Ok(listFiles)
            : NotFound("A página não existe...");
    }

    [HttpPost]
    public async Task<IActionResult> CreatePage([FromRoute] string title, [FromBody] CreatePageDto newPage)
    {
        string pin = await _gnPin.Generate(title);
        PageModel page = new(title, pin, newPage.Password);

        return ((pin != null) && (await _page.CreatePage(page)))
            ? Created("", page)
            : BadRequest("Falha ao criar página!");
    }

    /*
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    */
}
