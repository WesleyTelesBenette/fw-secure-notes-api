using fw_secure_notes_api.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace fw_secure_notes_api.Services
{
    public class GenerateTokenService
    {
        private readonly IConfiguration _config;

        public GenerateTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string title, string pin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);
            var tokenDescripto = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, $"{title}-{pin}"),
                    new(ClaimTypes.Role, "user"),
                    new("Title", title),
                    new("Pin", pin)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescripto);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
