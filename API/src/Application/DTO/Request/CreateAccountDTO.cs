﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request;
public class CreateAccountDTO
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}
