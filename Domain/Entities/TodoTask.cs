using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class TodoTask : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid MeetingId { get; set; }
    [ForeignKey("MeetingId")] 
    public Meeting Meeting { get; set; } = null!;
    
    public bool IsCompleted { get; set; }
    
    public required string Title { get; set; }
}