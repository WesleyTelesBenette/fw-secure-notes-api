using fw_secure_notes_api.Attributes;
using fw_secure_notes_api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("Dev")]
public class DevController(DatabaseContext db) : ControllerBase 
{
    private DatabaseContext _db = db;

    [HttpGet("pages")]
    [NoParameters]
    public async Task<IActionResult> GetAllPages()
    {
        var pages = await _db.Pages.ToListAsync();
        return Ok(pages);
    }

    [HttpGet("files")]
    [NoParameters]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _db.Files.ToListAsync();
        return Ok(files);
    }


    [HttpDelete("all")]
    [NoParameters]
    public IActionResult DeleteAll()
    {
        try
        {
            var allPages = _db.Pages.ToList();

            _db.Pages.RemoveRange(allPages);
            _db.SaveChanges();  

            return Ok();
        }
        catch(Exception e)
        {
            return BadRequest(e);
        }
        
    }
}
