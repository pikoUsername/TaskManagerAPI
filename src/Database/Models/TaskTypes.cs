﻿using System.ComponentModel.DataAnnotations;

namespace TaskManager.Database.Models;

public class TaskType
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}