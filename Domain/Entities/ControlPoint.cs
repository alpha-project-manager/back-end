using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities;

public class ControlPoint : IHasId
{
    [Key]
    public required Guid Id { get; set; }

    public string Title { get; set; } = null!;
    
    public required DateTime Date { get; set; }
}