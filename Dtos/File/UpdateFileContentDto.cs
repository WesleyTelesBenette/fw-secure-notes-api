using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos.File;

public class UpdateFileContentDto
{
    [Required]
    public int Index { get; set; }

    public string? Content { get; set; } = null;
}
