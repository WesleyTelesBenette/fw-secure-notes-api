namespace fw_secure_notes_api.Dtos;

public class FileModelDto
{
    public string Title { get; set; } = string.Empty;

    public List<string> Content { get; set; } = [];
}
