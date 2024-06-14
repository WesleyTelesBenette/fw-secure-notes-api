namespace fw_secure_notes_api.Dtos.File;

public class FileModelDto
{
    public ushort Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<string> Content { get; set; } = [];
}
