using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class LoginDto
{
    [Required]
    [MaxLength(25)]
    public string Title { get; set; }

    [Required]
    [MaxLength(3)]
    [RegularExpression(@"^[a-zA-Z0-9\-]*$")]
    public string Pin { get; set; }

    [Required]
    public string Password { get; set; }
}
