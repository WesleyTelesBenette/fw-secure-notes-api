using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : Controller
{
    private readonly PageRepository _page;
    private readonly GenerateTokenService _gnrtToken;

    public AuthenticationController
        (PageRepository pageRepository, GenerateTokenService gnrtToken)
    {
        _page = pageRepository;
        _gnrtToken = gnrtToken;
    }

    [HttpGet]
    public async Task<IActionResult> IsPageHasPassword([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            return (await _page.IsPageExist(title, pin))
                ? Ok(await _page.IsPageHasPassword(title, pin))
                : NotFound("A página não existe...");
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = "Ocorreu um erro inesperado no servidor.",
                details = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken
        ([FromRoute] string title,
        [FromRoute] string pin,
        [FromBody] LoginDto login)
    {
        try
        {
            if (!await _page.IsPageExist(title, pin))
                return NotFound("A página não existe...");

            if (!await _page.IsPageValid(title, pin, login.Password))
                return Unauthorized("A senha está incorreta!");

            string token = _gnrtToken.GenerateToken(title, pin);

            return Ok(new { Token = token });
        }
        catch(Exception e)
        {
            return StatusCode(500, new
            {
                message = "Ocorreu um erro inesperado no servidor.",
                details = e.Message
            });
        }
    }
}
