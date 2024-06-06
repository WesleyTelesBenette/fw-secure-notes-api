using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : Controller
{
    private readonly FileRepository _file;
    private readonly ActionResultService _result;

    public FileController(FileRepository file, ActionResultService result)
    {
        _file = file;
        _result = result; 
    }

    [HttpGet("{fileIndex}")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> GetFile([FromRoute] string title, [FromRoute] string pin, [FromRoute] int fileIndex)
    {
        try
        {
            var file = await _file.GetFile(title, pin, fileIndex);

            return (file != null)
                ? _result.GetAction(ActionResultService.Results.Get, content: file)
                : _result.GetActionAuto(ActionResultService.Results.NotFound, $"File[{fileIndex}]");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPost]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> CreateFile([FromRoute] string title, [FromRoute] string pin, [FromBody] CreateFileDto newFile)
    {
        try
        {
            var result = await _file.CreateFile(title, pin, newFile.Title);

            return _result.GetActionAuto(result, $"File({newFile.Title})");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("{fileIndex}/title")]
    public async Task<IActionResult> UpdateFileTitle
        ([FromRoute] string title, [FromRoute] string pin, [FromRoute] int fileIndex, [FromBody] UpdateFileTitleDto newTitle)
    {
        try
        {
            var result = await _file.UpdateFileTitle(title, pin, fileIndex, newTitle.Title);

            return _result.GetActionAuto(result, $"File[{fileIndex}].Title");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPut("{fileIndex}/content")]
    public async Task<IActionResult> UpdateFileContent
        ([FromRoute] string title, [FromRoute] string pin, int fileIndex, [FromBody] UpdateFileContentDto updateContent)
    {
        try
        {
            var result = await _file.UpdateFileContent(title, pin, fileIndex, updateContent.Content);

            return _result.GetActionAuto(result, $"File[{fileIndex}].Content");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpDelete("{fileIndex}")]
    [ServiceFilter(typeof(TokenValidateActionFilter))]
    public async Task<IActionResult> DeleteFile([FromRoute] string title, [FromRoute] string pin, [FromRoute] ushort fileIndex)
    {
        try
        {
            var result = await _file.DeleteFile(title, pin, fileIndex);

            return _result.GetActionAuto(result, $"File[{fileIndex}]");
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }
}
