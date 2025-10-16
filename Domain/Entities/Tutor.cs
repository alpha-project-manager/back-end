using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities;

public class Tutor : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";
}