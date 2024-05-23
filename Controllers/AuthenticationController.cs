using fw_secure_notes_api.Data;
using fw_secure_notes_api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fw_secure_notes_api.Controllers;

public class AuthenticationController : Controller
{
    private readonly PageRepository _page;
    private readonly IConfiguration _configuration;

    public AuthenticationController(PageRepository pageRepository, IConfiguration configuration)
    {
        _page = pageRepository;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken(LoginDto login)
    {
        try
        {
            bool isValidPage = await _page.IsPageValid(login.Title, login.Pin, login.Password);

            if (!isValidPage)
            {
                return Unauthorized("A senha está incorreta!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim("title", login.Title),
                new Claim("pin", login.Pin)
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
