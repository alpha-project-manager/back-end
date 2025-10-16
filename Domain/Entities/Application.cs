using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Application : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid CaseId { get; set; }
    [ForeignKey("CaseId")] 
    public ProjectCase ProjectCase { get; set; } = null!;
    
    public required string TeamTitle { get; set; }
    
    public required ApplicationStatus Status { get; set; }

    public string MembersInfo { get; set; } = null!;

    public string TeamInfo { get; set; } = null!;
}