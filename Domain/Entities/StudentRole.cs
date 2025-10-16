﻿using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities;

public class StudentRole : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
}