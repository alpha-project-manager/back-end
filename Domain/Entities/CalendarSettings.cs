using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities;

public class CalendarSettings : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string ServerUrl { get; set; }
    
    public string? Login { get; set; }

    public string? Password { get; set; }
}