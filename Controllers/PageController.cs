using fw_secure_notes_api.Data;
using fw_secure_notes_api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(TokenValidateActionFilter))]
public class PageController : Controller
{
    private readonly PageRepository _page;

    public PageController(PageRepository page)
    {
        _page = page;
    }

    [HttpGet("files")]
    public async Task<IActionResult> GetFileList([FromRoute] string title, [FromRoute] string pin)
    {
        var listFiles = await _page.GetFileList(title, pin);

        return (listFiles.Count > 0)
            ? Ok(listFiles)
            : NotFound();
    }

    /*[HttpPost]
    public async Task<IActionResult> CreatePage()
    {

    }*/

    /*
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    public async Task<IActionResult> () { }
    */
}
