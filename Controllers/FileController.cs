using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : Controller
{
    private readonly FileRepository _file;

    public FileController(FileRepository file)
    {
        _file = file;
    }

    [HttpGet("{fileIndex}")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetFileContent([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileIndex)
    {
        var file = await _file.GetFileContent(title, pin, fileIndex);

        return Ok(file);
    }

    [HttpPost]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> CreateFile([FromRoute] string title, [FromRoute] string pin, [FromBody] CreateFileDto newFile)
    {
        var create = await _file.CreateFile(title, pin, newFile.Title);

        return (create)
            ? Ok()
            : BadRequest();
    }

    [HttpPut("{fileIndex}/title")]
    public async Task<IActionResult> UpdateFileTitle
        ([FromRoute] string title, [FromRoute] string pin, [FromRoute] int fileIndex, [FromBody] UpdateFileTitleDto newTitle)
    {
        var update = await _file.UpdateFileTitle(title, pin, fileIndex, newTitle.Title);

        return (update)
            ? Ok()
            : BadRequest();
    }

    [HttpPut("{fileIndex}/content")]
    public async Task<IActionResult> UpdateFileContent
        ([FromRoute] string title, [FromRoute] string pin, int fileIndex, [FromBody] UpdateFileContentDto updateContent)
    {
        var update = await _file.UpdateFileContent(title, pin, fileIndex, updateContent.Content);

        return (update)
            ? Ok()
            : BadRequest();
    }


    [HttpDelete("{fileIndex}")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeleteFile([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileIndex)
    {
        var delete = await _file.DeleteFile(title, pin, fileIndex);
        return (delete)
            ? Ok()
            : BadRequest();
    }
}
