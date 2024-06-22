using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos.File;

public class UpdateFileContentDto
{
    [Required]
    public string Content { get; set; } = "";
}
