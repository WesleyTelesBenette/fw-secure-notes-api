using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class DeletePageDto
{
    [Required]
    public string Password { get; set; } = "";
}
