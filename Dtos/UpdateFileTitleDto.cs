using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class UpdateFileTitleDto
{
    [Required]
    public string Title { get; set; } = "";
}
