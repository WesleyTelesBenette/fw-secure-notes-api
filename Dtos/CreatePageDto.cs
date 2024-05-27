using fw_secure_notes_api.Models;
using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class CreatePageDto
{
    public string Password { get; set; } = "";
}
