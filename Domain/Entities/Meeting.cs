using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class Meeting : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid ProjectId { get; set; }
    [ForeignKey("ProjectId")] 
    public Project Project { get; set; } = null!;

    public string Description { get; set; } = "";

    public int? ResultMark { get; set; } = null;
    
    public required bool IsFinished { get; set; }
    
    public required DateTime DateTime { get; set; }
}