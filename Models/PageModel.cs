using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Models;

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

    public PageModel(string title, string pin, string password)
    {
        Title = title;
        Pin = pin;
        Password = password;
    }
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
