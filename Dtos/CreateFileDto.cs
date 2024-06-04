﻿using System.ComponentModel.DataAnnotations;

namespace fw_secure_notes_api.Dtos;

public class CreateFileDto
{
    [MaxLength(48)]
    public string Title { get; set; } = "File Default...";
}
