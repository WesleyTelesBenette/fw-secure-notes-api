using fw_secure_notes_api.Models;
using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class UpdatePageThemeDto
{
    [Required]
    public ThemePage Theme { get; set; } = ThemePage.Dark;
}

