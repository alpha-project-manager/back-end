using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Project : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public Guid? CaseId { get; set; }
    [ForeignKey("CaseId")] 
    public ProjectCase Case { get; set; } = null!;
    
    public required string TeamTitle { get; set; }
    
    public required string Title { get; set; }

    public string Description { get; set; } = "";
    
    public Guid? TutorId { get; set; }
    [ForeignKey("TutorId")]
    public Tutor? Tutor { get; set; }

    public string MeetingUrl { get; set; } = "";

    public required ProjectStatus Status { get; set; }
    
    public required Semester Semester { get; set; }
    
    public required int AcademicYear { get; set; }
}