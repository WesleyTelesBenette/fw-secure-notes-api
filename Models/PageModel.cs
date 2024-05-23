using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Models;

public class PageModel
{
    [Key]
    public uint Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
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
