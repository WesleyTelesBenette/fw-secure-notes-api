using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos.File;

public class UpdateFileContentDto
{
    [Required]
    public Dictionary<int, string?> Content { get; set; }
}
