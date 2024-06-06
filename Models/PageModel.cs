using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fw_secure_notes_api.Models;

[Table("Page")]
public class PageModel
{
    [Key]
    public uint Id { get; set; }

    [Required]
    [MaxLength(25)]
    public string Title { get; set; }

    [Required]
    [MaxLength(3)]
    [RegularExpression(@"^[a-zA-Z0-9\-]*$")]
    public string Pin { get; set; }

    public string Password { get; set; } = "";

    public ThemePage Theme { get; set; } = ThemePage.Dark;

    public ICollection<FileModel> Files { get; set; } = [];

    public PageModel() { }
}

public enum ThemePage
{
    Dark,
    Light,
    Red,
    Gold,
    Blue,
    Green,
    Purple,
    Orange
}
