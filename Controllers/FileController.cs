using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos.File;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(ParmatersValidateActionFilter))]
[ServiceFilter(typeof(TokenValidateActionFilter))]
public class FileController : Controller
{
    private readonly FileRepository _file;
    private readonly ActionResultService _result;

    public FileController(FileRepository file, ActionResultService result)
    {
        _file = file;
        _result = result; 
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFile([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileId)
    {
        try
        {
            var file = await _file.GetFile(title, pin, fileId);

            return (file != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: file)
                : _result.GetActionAuto(ActionResultService.Results.NotFound, $"File[{fileId}]");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateFile([FromRoute] string title, [FromRoute] string pin, [FromBody] CreateFileDto newFile)
    {
        try
        {
            var file = await _file.CreateFile(title, pin, newFile.Title);

            return (file != null)
                ? _result.GetActionAuto(ActionResultService.Results.Created, content: file)
                : _result.GetActionAuto(ActionResultService.Results.Bad, $"File({newFile.Title})");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("{fileId}/title")]
    public async Task<IActionResult> UpdateFileTitle
        ([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileId, [FromBody] UpdateFileTitleDto newTitle)
    {
        try
        {
            var result = await _file.UpdateFileTitle(title, pin, fileId, newTitle.Title);

            return _result.GetActionAuto(result, $"File[{fileId}].Title");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("{fileId}/content")]
    public async Task<IActionResult> UpdateFileContent
        ([FromRoute] string title, [FromRoute] string pin, ushort fileId, [FromBody] UpdateFileContentDto updateContent)
    {
        try
        {
            var result = await _file.UpdateFileContent(title, pin, fileId, updateContent);

            return _result.GetActionAuto(result, $"File[{fileId}].Content");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileId)
    {
        try
        {
            var result = await _file.DeleteFile(title, pin, fileId);

            return _result.GetActionAuto(result, $"File[{fileId}]");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }
}
