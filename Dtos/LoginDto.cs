using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class LoginDto
{
    [Required]
    public required string Password { get; set; }
}
