using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos.File;

public class UpdateFileTitleDto
{
    [MaxLength(48)]
    public string Title { get; set; } = "";
}
