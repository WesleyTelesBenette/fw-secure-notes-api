using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fw_secure_notes_api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : Controller
{
    private readonly PageRepository _page;
    private readonly IConfiguration _configuration;

    public AuthenticationController(PageRepository pageRepository, IConfiguration configuration)
    {
        _page = pageRepository;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> IsPageHasPassword([FromRoute] string title, [FromRoute] string pin)
    {
        return (await _page.IsPageExist(title, pin))
            ? Ok(await _page.IsPageHasPassword(title, pin))
            : NotFound();
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

            if (!await _page.IsPageValid(title, pin, login.Password ?? ""))
                return Unauthorized("A senha está incorreta!");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new("title", title),
                    new("pin", pin)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenString = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return Ok(new { Token = tokenString });
        }
        catch (SecurityTokenException e)
        {
            return BadRequest(new
            {
                message = "Falha ao gerar token de autorização.",
                details = e.Message
            });
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, new
            {
                message = "Erro ao acessar o banco de dados.",
                details = e.Message
            });
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
