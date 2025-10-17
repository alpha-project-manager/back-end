using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class ProjectCase : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public string Description { get; set; } = "";
    
    public string Goal { get; set; } = "";
    
    public string RequestedResult { get; set; } = "";

    public string Criteria { get; set; } = "";
    
    public Guid? TutorId { get; set; }
    [ForeignKey("TutorId")]
    public Tutor? Tutor { get; set; }
    
    public required int MaxTeams { get; set; }
    
    public required int AcceptedTeams { get; set; }
    
    public required bool IsActive { get; set; }
}