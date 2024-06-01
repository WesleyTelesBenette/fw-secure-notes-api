using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class CreatePageDto
{
    [Required]
    public string Title { get; set; }
    public string Password { get; set; } = "";
}
