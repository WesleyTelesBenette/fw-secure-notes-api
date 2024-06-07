namespace fw_secure_notes_api.Dtos;

public class ResultDto
{
    public string? Message { get; set; } = null;
    public int StatusCode { get; set; }
    public object? Content { get; set; } = null;
}
