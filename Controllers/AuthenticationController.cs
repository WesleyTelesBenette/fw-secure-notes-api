using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using fw_secure_notes_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : Controller
{
    private readonly AuthenticationRepository _auth;
    private readonly GenerateTokenService _gnrtToken;
    private readonly ActionResultService _result;

    public AuthenticationController
        (AuthenticationRepository authenticationRepository, GenerateTokenService gnrtToken, ActionResultService result)
    {
        _auth = authenticationRepository;
        _gnrtToken = gnrtToken;
        _result = result;
    }

    [HttpGet]
    public async Task<IActionResult> GetPageHasPassword([FromRoute] string title, [FromRoute] string pin)
    {
        try
        {
            var isPassword = await _auth.GetPageHasPassword(title, pin);

            return _result.GetAction(ActionResultService.Results.Get, content: isPassword);
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken
        ([FromRoute] string title,
        [FromRoute] string pin,
        [FromBody] LoginDto login)
    {
        try
        {
            var result = await _auth.IsPageValide(title, pin, login.Password);
            string token = _gnrtToken.GenerateToken(title, pin);

            return _result.GetAction(result, content: $"Bearer {token}"); ;
        }
        catch (Exception e)
        {
            return _result.GetActionAuto(ActionResultService.Results.ServerError, content: e);
        }
    }
}
