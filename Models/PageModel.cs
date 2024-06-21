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
    [RegularExpression(@"^[a-zA-Z0-9\-]+$")]
    public string Pin { get; set; }

    public string Password { get; set; } = "";

    public int Theme { get; set; } = 0;

    public ICollection<FileModel> Files { get; set; } = [];

    public PageModel() { }
}
