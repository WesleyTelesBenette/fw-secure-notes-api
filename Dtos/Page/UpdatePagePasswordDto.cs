namespace fw_secure_notes_api.Dtos;

public class UpdatePagePasswordDto
{
    public string OldPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}
