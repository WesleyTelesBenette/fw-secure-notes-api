using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Models;

[Table("File")]
public class FileModel
{
    [Key]
    public ushort Id { get; set; }

    [MaxLength(24)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [ForeignKey(nameof(FileModel.Page))]
    public uint PageId { get; set; }

    public PageModel Page { get; set; }

    public FileModel() { }
}
